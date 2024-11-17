using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monero.Api.Data;
using Monero.Api.Models;
using Monero.Api.Services;
using Monero.Models;
using Monero.Wallet;
using Monero.Wallet.Responses;
using System.Collections.Concurrent;

namespace Monero.Api.Singletons;

public class PaymentSingleton
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly IServiceProvider _serviceProvider;
    private readonly ManualResetEventSlim _orderUpdatesAvailable = new(false);
    private readonly ConcurrentQueue<OrderUpdate> _updatedOrders = [];
    private readonly ConcurrentQueue<string> _transactionHashes = [];
    private readonly ILogger<PaymentSingleton> _logger;

    private CancellationTokenSource _cancellationTokenSource = new();

    public PaymentSingleton(IServiceProvider serviceProvider, ILogger<PaymentSingleton> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        Task.Run(ProcessTransactions);
        Task.Run(ProcessUpdatedOrders);
    }

    public Task QueueTransactionAsync(string transactionHash)
    {
        _transactionHashes.Enqueue(transactionHash);
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }

    private async Task ProcessTransactions()
    {
        while (true)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var moneroWalletService = scope.ServiceProvider.GetRequiredService<IMoneroWalletService>();
                var db = scope.ServiceProvider.GetRequiredService<MoneroApiContext>();
                var settings = scope.ServiceProvider.GetRequiredService<IOptions<MoneroSettings>>().Value;

                bool anOrderWasChanged = false;
                var utcNow = DateTime.UtcNow;

                var expiredPayments = db.Payments.Where(x => x.PaymentState == PaymentState.Unpaid && utcNow > x.ValidUntil);

                await expiredPayments.ForEachAsync(x => {
                    x.PaymentState = PaymentState.Expired;
                    x.UpdatedAt = utcNow;

                    _updatedOrders.Enqueue(new OrderUpdate { OrderId = x.OrderId, StatusId = 5 });

                    anOrderWasChanged = true;
                    Console.Write(x.Id + " Expired");
                });

                await moneroWalletService.RefreshWalletAsync();

                // Get pending transactions here and check their confirmations
                var pendingPayments = db.Payments.Where(x => x.PaymentState == PaymentState.Pending);
                foreach (var pendingPayment in pendingPayments)
                {
                    var transfersResponse = await moneroWalletService.GetPendingAndIncomingTransfersAsync(0, [pendingPayment.AddressIndex]);
                    if (transfersResponse is null)
                    {
                        _logger.LogError(new Exception($"Could not get incoming transfers for pending payment {pendingPayment.Id}"), "");
                        continue;
                    }

                    ulong totalPaidAmount = 0;

                    foreach (var incomingTransfer in transfersResponse.IncomingTransfers)
                    {
                        if (incomingTransfer.Confirmations >= settings.Confirmations)
                        {
                            totalPaidAmount += incomingTransfer.Amount;
                        }
                    }

                    if (totalPaidAmount >= pendingPayment.Piconero)
                    {
                        Console.WriteLine(pendingPayment.Id + " Paid");
                        pendingPayment.PaymentState = PaymentState.Paid;

                        _updatedOrders.Enqueue(new OrderUpdate { OrderId = pendingPayment.OrderId, StatusId = 11 });
                        anOrderWasChanged = true;
                    }
                }

                // Transactions that were just seen (0-conf) and transactions considered completed by the wallet rpc
                while (_transactionHashes.TryDequeue(out var transactionHash))
                {
                    Console.WriteLine("Processing transaction: " + transactionHash);

                    var transferResponse = await moneroWalletService.GetTransferByTxIdAsync(transactionHash);
                    if (transferResponse is null)
                    {
                        _logger.LogError(new Exception($"Could not get transfer for transaction hash {transactionHash}"), "");
                        continue;
                    }

                    var transfer = transferResponse.Transfer;

                    var payment = await db.Payments.FirstOrDefaultAsync(x => x.AddressIndex == transfer.SubAddressIndex.Minor);
                    if (payment is null)
                        continue;

                    // Double check "pool"
                    if (transfer.UnlockTime != 0 || transfer.DoubleSpendSeen || (transfer.Type != "in" && transfer.Type != "pool"))
                        continue;

                    if (transfer.Amount < payment.Piconero)
                    {
                        // If amount is less than full amount we need to get potential other payments
                        var transfersResponse = await moneroWalletService.GetPendingAndIncomingTransfersAsync(0, [transfer.SubAddressIndex.Minor]);
                        if (transfersResponse is null)
                        {
                            _logger.LogError(new Exception($"Could not get incoming transfers for payment {payment.Id}"), "");
                            continue;
                        }

                        ulong totalPaidAmount = 0;
                        ulong totalPendingAmount = 0;

                        foreach (var partialTransfer in transfersResponse.IncomingTransfers)
                        {
                            if (transfer.Confirmations < settings.Confirmations)
                            {
                                totalPendingAmount += partialTransfer.Amount;
                            }
                            else
                            {
                                totalPaidAmount += partialTransfer.Amount;
                            }
                        }

                        if (totalPaidAmount >= payment.Piconero)
                        {
                            if (payment.PaymentState == PaymentState.Unpaid || payment.PaymentState == PaymentState.Pending)
                            {
                                Console.WriteLine(payment.Id + " Paid");
                                payment.PaymentState = PaymentState.Paid;

                                _updatedOrders.Enqueue(new OrderUpdate { OrderId = payment.OrderId, StatusId = 11 });
                                anOrderWasChanged = true;
                            }
                        }
                        else if (totalPendingAmount >= payment.Piconero)
                        {
                            if (payment.PaymentState == PaymentState.Unpaid)
                            {
                                Console.WriteLine(payment.Id + " Pending");
                                payment.PaymentState = PaymentState.Pending;
                            }
                        }
                    }
                    else // If amount was paid in full from a single transaction
                    {
                        if (transfer.Confirmations < settings.Confirmations)
                        {
                            if (payment.PaymentState == PaymentState.Unpaid)
                            {
                                Console.WriteLine(payment.Id + " Pending");
                                payment.PaymentState = PaymentState.Pending;
                            }
                        }
                        else
                        {
                            if (payment.PaymentState == PaymentState.Unpaid || payment.PaymentState == PaymentState.Pending)
                            {
                                Console.WriteLine(payment.Id + " Paid");
                                payment.PaymentState = PaymentState.Paid;

                                _updatedOrders.Enqueue(new OrderUpdate { OrderId = payment.OrderId, StatusId = 11 });
                                anOrderWasChanged = true;
                            }
                        }
                    }

                }

                await db.SaveChangesAsync();

                if (anOrderWasChanged)
                {
                    _orderUpdatesAvailable.Set();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }

            try
            {
                await Task.Delay(30_000, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                _cancellationTokenSource = new();
            }
        }
    }

    public async Task ProcessTransactionAsync(string transactionHash)
    {
        await _semaphore.WaitAsync();
        try
        {
            Console.WriteLine("New transaction: " + transactionHash);

            using var scope = _serviceProvider.CreateScope();
            var moneroWalletService = scope.ServiceProvider.GetRequiredService<IMoneroWalletService>();
            var db = scope.ServiceProvider.GetRequiredService<MoneroApiContext>();
            var settings = scope.ServiceProvider.GetRequiredService<IOptions<MoneroSettings>>().Value;

            var utcNow = DateTime.UtcNow;
            var expiredPayments = db.Payments.Where(x => x.PaymentState == PaymentState.Unpaid && utcNow > x.ValidUntil);

            bool anOrderWasChanged = false;

            await expiredPayments.ForEachAsync(x => {
                x.PaymentState = PaymentState.Expired;

                _updatedOrders.Enqueue(new OrderUpdate { OrderId = x.OrderId, StatusId = 5 });

                anOrderWasChanged = true;
                Console.Write(x.Id + " Expired");
            });

            var unpaidAndPendingPayments = await db.Payments.Where(x => x.PaymentState == PaymentState.Unpaid || x.PaymentState == PaymentState.Pending).OrderBy(x => x.AddressIndex).ToListAsync();
            if (unpaidAndPendingPayments.Count == 0)
            {
                if (anOrderWasChanged)
                {
                    await db.SaveChangesAsync();
                    _orderUpdatesAvailable.Set();
                }

                return;
            }

            var indices = unpaidAndPendingPayments.Select(x => x.AddressIndex).ToArray();
            if (indices.Length == 0)
                return;

            List<Transfer> incomingTransfers = [];

            await moneroWalletService.RefreshWalletAsync();

            for (int i = 0; i < 3; i++)
            {
                // This will get ALL transfers that are associated with a pending or unpaid payment
                var transfersResponse = await moneroWalletService.GetPendingAndIncomingTransfersAsync(0, indices);
                if (transfersResponse is null)
                {
                    if (i == 2)
                        throw new Exception("Did not get a valid/any response from wallet rpc.");

                    continue;
                }

                incomingTransfers = transfersResponse.IncomingTransfers;
                break;
            }

            if (incomingTransfers.Count == 0)
                return;

            Dictionary<uint, Payment> unpaidAndPendingPaymentsHashSet = unpaidAndPendingPayments.Select(x => (x.AddressIndex, x)).ToDictionary();

            // GroupBy in case a payment is sent in multiple payments
            var confirmedIncomingTransfers = incomingTransfers
                .Where(x => x.UnlockTime == 0 && !x.DoubleSpendSeen && x.Confirmations >= settings.Confirmations)
                .GroupBy(x => x.SubAddressIndex.Minor)  // Group by address index
                .Select(x => new
                {
                    AddressIndex = x.Key,
                    Amount = x.Select(y => y.Amount).Aggregate((a, b) => a + b),
                });

            foreach (var pair in confirmedIncomingTransfers)
            {
                var payment = unpaidAndPendingPaymentsHashSet[pair.AddressIndex];
                if ((payment.PaymentState == PaymentState.Pending || payment.PaymentState == PaymentState.Unpaid) && pair.Amount >= payment.Piconero)
                {
                    Console.WriteLine(payment.Id + " Paid");
                    payment.PaymentState = PaymentState.Paid;

                    _updatedOrders.Enqueue(new OrderUpdate { OrderId = payment.OrderId, StatusId = 11 });
                    anOrderWasChanged = true;
                }
            }

            // 
            var pendingIncomingTransfers = incomingTransfers
                .Where(x => x.UnlockTime == 0 && !x.DoubleSpendSeen && x.Confirmations < settings.Confirmations)
                .GroupBy(x => x.SubAddressIndex.Minor)
                .Select(x => new
                {
                    AddressIndex = x.Key,
                    Amount = x.Select(y => y.Amount).Aggregate((a, b) => a + b),
                });

            foreach (var pair in pendingIncomingTransfers)
            {
                var payment = unpaidAndPendingPaymentsHashSet[pair.AddressIndex];

                if (payment.PaymentState == PaymentState.Unpaid && pair.Amount >= payment.Piconero)
                {
                    Console.WriteLine(payment.Id + " Pending");
                    payment.PaymentState = PaymentState.Pending;
                }
            }

            await db.SaveChangesAsync();

            if (anOrderWasChanged)
            {
                _orderUpdatesAvailable.Set();
            }

            Console.WriteLine("Finished");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task ProcessUpdatedOrders()
    {
        while (true) 
        {
            try
            {
                _orderUpdatesAvailable.Wait();

                using var scope = _serviceProvider.CreateScope();
                var bigCommerceService = scope.ServiceProvider.GetRequiredService<IBigCommerceService>();

                while (_updatedOrders.TryDequeue(out var orderUpdate))
                {
                    for (var i = 0; i < 3; i++) 
                    { 
                        var response = await bigCommerceService.UpdateOrderAsync(orderUpdate);
                        if (response.Value is not null)
                            break;

                        if (i == 2)
                        {
                            _logger.LogError(new Exception($"Could not update order {orderUpdate.OrderId}"), "");
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                _logger.LogError(e, "");
            }
            finally
            {
                _orderUpdatesAvailable.Reset();
            }
        }
    }
}
