using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using System.Net;
using System.Runtime.InteropServices;
using Monero.Wallet.Requests;
using Monero.Wallet.Responses;
using Monero.Models;
using Monero.Shared;

namespace Monero.Wallet;

public class MoneroWalletService : IMoneroWalletService
{
    private readonly MoneroRPCClient _moneroRPCClient;
    private readonly MoneroSettings _settings;
    private readonly string _daemonAddress;

    public MoneroWalletService(HttpClient httpClient, IOptions<MoneroSettings> options)
    {
        var port = options.Value.Network switch
        {
            Network.Mainnet => 18082,
            Network.Testnet => 28082,
            Network.Stagenet => 38082,
            _ => throw new ArgumentException("Not a valid network."),
        };

        if (options.Value.UseRemoteNode)
        {
            if (!IPAddress.TryParse(options.Value.DaemonAddress, out var _))
                throw new ArgumentException("DaemonAddress is not a valid address.");

            _daemonAddress = options.Value.DaemonAddress;
        }
        else
        {
            _daemonAddress = "127.0.0.1";
        }

        _settings = options.Value;
        _moneroRPCClient = new MoneroRPCClient(httpClient, "127.0.0.1", port);
    }

    public async Task<string?> PayFromMasterAsync(ulong amount, string toAddress)
    {
        var response = await TransferAsync(amount, 0, toAddress, [], true);

        return response?.TxHash;
    }

    public async Task<CreateAddressResponse?> CreateAddressAsync(uint accountIndex)
    {
        var request = new ExtendedBaseRequest<CreateAddressRequest>("create_address");
        request.Params.AccountIndex = accountIndex;

        var response = await _moneroRPCClient.GetAsync<CreateAddressResponse>(request);

        if (response is null || response.Result is null)
            return default;

        var savedSuccessfully = await SaveWalletAsync();

        return response.Result;
    }

    public async Task<GetFeeResponse?> GetRealFeeAsync(ulong amount, uint fromAccount, string toAddress, uint[] subbaddressIndices)
    {
        var request = new ExtendedBaseRequest<TransferRequest>("transfer");
        request.Params.AccountIndex = fromAccount;
        request.Params.DoNotRelay = true;
        request.Params.Destinations = [new Destination { Address = toAddress, Amount = amount }];
        request.Params.SubaddressIndices = subbaddressIndices;

        var response = await _moneroRPCClient.GetAsync<TransferResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return new GetFeeResponse { Fee = response.Result.Fee, TxMetadata = response.Result.TxMetadata };
    }

    public async Task<RelayTxResponse?> RelayTxAsync(string hex)
    {
        var request = new ExtendedBaseRequest<RelayTxRequest>("relay_tx");
        request.Params.Hex = hex;

        var response = await _moneroRPCClient.GetAsync<RelayTxResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }

    public async Task<TransferResponse?> TransferAsync(ulong amount, uint fromAccountIndex, string toAddress, uint[] subbaddressIndices, bool subtractFee)
    {
        if (subtractFee)
        {
            var feeResponse = await GetRealFeeAsync(amount, fromAccountIndex, toAddress, subbaddressIndices);

            if (feeResponse is not null)
            {
                // TODO make configable
                if (feeResponse.Fee > 505_400_000)
                {
                    return default;
                }

                var minusFeeRequest = new ExtendedBaseRequest<TransferRequest>("transfer");
                minusFeeRequest.Params.AccountIndex = fromAccountIndex;
                minusFeeRequest.Params.SubaddressIndices = subbaddressIndices;
                minusFeeRequest.Params.Destinations = [new Destination { Address = toAddress, Amount = amount - feeResponse.Fee }];

                var minusFeeResponse = await _moneroRPCClient.GetAsync<TransferResponse>(minusFeeRequest);

                if (minusFeeResponse is null || minusFeeResponse.Result is null)
                    return default;

                return minusFeeResponse.Result;
            }
            else return default;
        }

        var request = new ExtendedBaseRequest<TransferRequest>("transfer");
        request.Params.Destinations = [new Destination { Address = toAddress, Amount = amount }];
        request.Params.AccountIndex = fromAccountIndex;
        request.Params.SubaddressIndices = subbaddressIndices;

        var response = await _moneroRPCClient.GetAsync<TransferResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }

    public async Task<CreateAccountResponse?> CreateAccountAsync()
    {
        var response = await _moneroRPCClient.GetAsync<CreateAccountResponse>(new ExtendedBaseRequest<CreateAccountRequest>("create_account"));

        if (response is null || response.Result is null)
            return default;

        var savedSuccessfully = await SaveWalletAsync();

        return response.Result;
    }

    public async Task<GetBalanceResponse?> GetBalanceAsync(uint accountIndex, uint[] indices)
    {
        var request = new ExtendedBaseRequest<GetBalanceRequest>("get_balance");
        request.Params.AccountIndex = accountIndex;
        request.Params.AddressIndices = indices;

        var response = await _moneroRPCClient.GetAsync<GetBalanceResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }

    public async Task SetDaemonAsync()
    {
        var response = await _moneroRPCClient.GetAsync<object>(new ExtendedBaseRequest<object>("set_daemon"));
    }

    public async Task<GetAddressResponse?> GetAddressAsync()
    {
        var request = new ExtendedBaseRequest<GetAddressRequest>("get_address");

        var response = await _moneroRPCClient.GetAsync<GetAddressResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }

    public async Task<EstimateTxSizeAndWeightResponse?> GetSizeAndWeightEstimateAsync()
    {
        var request = new ExtendedBaseRequest<EstimateTxSizeAndWeightRequest>("estimate_tx_size_and_weight");

        var response = await _moneroRPCClient.GetAsync<EstimateTxSizeAndWeightResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }

    public async Task<bool> CreateWalletAsync(string walletName)
    {
        var request = new ExtendedBaseRequest<CreateWalletRequest>("create_wallet");
        request.Params.Filename = walletName;

        var response = await _moneroRPCClient.GetAsync<object>(request);

        if (response is null || !string.IsNullOrEmpty(response.Error?.Message))
            return false;

        return true;
    }

    public async Task<bool> OpenWalletAsync(string walletName)
    {
        var request = new ExtendedBaseRequest<OpenWalletRequest>("open_wallet");
        request.Params.Filename = walletName;

        var response = await _moneroRPCClient.GetAsync<object>(request);

        if (response is null || !string.IsNullOrEmpty(response.Error?.Message))
            return false;

        return true;
    }

    public bool CheckIfWalletExists(string walletName)
    {
        try
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(currentDirectory))
                return false;

            return File.Exists(Path.Combine(currentDirectory, "Programs", "Wallets", walletName));
        }
        catch
        {
            return false;
        }
    }

    public bool StartWalletRPC()
    {
        try
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(currentDirectory))
                return false;

            // Kill monero-wallet-rpc if already running
            var procesess = Process.GetProcesses();
            foreach (var p in procesess)
            {
                var processName = p.ProcessName.ToLower();

                if (processName.CompareTo("monero-wallet-rpc") == 0)
                {
                    p.Kill();
                    break;
                }
            }

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            ProcessStartInfo startInfo = new()
            {
                FileName = isWindows ? Path.Combine(currentDirectory, "Programs", "monero-wallet-rpc.exe") : Path.Combine(currentDirectory, "Programs", "monero-wallet-rpc"),
                Arguments = $"{(_settings.Network != Network.Mainnet ? $"--{_settings.Network}" : "")} --daemon-address {_daemonAddress}:{_moneroRPCClient.Port - 1} --rpc-bind-port {_moneroRPCClient.Port} --wallet-dir {Path.Combine("Programs", "Wallets")} --disable-rpc-login {(_settings.UseRemoteNode ? "--untrusted-daemon" : "--trusted-daemon")} --no-dns --log-level 4 --tx-notify \"{Path.Combine("Programs", isWindows ? "Fetch.exe" : "Fetch")} http://localhost:5100/api/notifications?transactionHash=%s key={_settings.APIKey}\"",
                RedirectStandardOutput = true,
                WorkingDirectory = currentDirectory
            };

            var process = Process.Start(startInfo);

            if (process is null)
                return false;

            process.Exited += (sender, e) =>
            {
                Console.WriteLine("WALLET RPC EXITED");
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                process.Kill();
                process.Dispose();
            };

            using var reader = process.StandardOutput;
            while (true)
            {
                var line = reader.ReadLine();
                Console.WriteLine(line);

                if (line is null)
                    throw new Exception("Line was null in MoneroWalletClient.");

                if (line.Contains("Starting wallet RPC server"))
                {
                    Thread.Sleep(2000);
                    return true;
                }

            }
        }
        catch
        {
            return false;
        }
    }

    public async Task<GetAccountsResponse?> GetAccountsAsync()
    {
        var request = new ExtendedBaseRequest<GetAccountsRequest>("get_accounts");

        var response = await _moneroRPCClient.GetAsync<GetAccountsResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }

    public async Task<GetTransfersResponse?> GetPendingAndIncomingTransfersAsync(uint accountIndex, uint[] subAddressIndices)
    {
        var request = new ExtendedBaseRequest<GetTransfersRequest>("get_transfers");
        request.Params.SubAddressIndices = subAddressIndices;
        request.Params.AccountIndex = accountIndex;
        request.Params.In = true;
        request.Params.Pending = true;

        var response = await _moneroRPCClient.GetAsync<GetTransfersResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }

    public async Task<bool> SaveWalletAsync()
    {
        var response = await _moneroRPCClient.GetAsync<object>(new ExtendedBaseRequest<object>("store"));

        if (response is null || response.Result is null)
            return false;

        return true;
    }

    public async Task<SweepAllResponse?> SweepAllAsync(uint fromAccount, string toAddress)
    {
        var request = new ExtendedBaseRequest<SweepAllRequest>("sweep_all");
        request.Params.Address = toAddress;
        request.Params.AccountIndex = fromAccount;

        var response = await _moneroRPCClient.GetAsync<SweepAllResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }

    public async Task<string?> GetSpendKeyAsync()
    {
        var request = new ExtendedBaseRequest<QueryKeyRequest>("query_key");

        var response = await _moneroRPCClient.GetAsync<QueryKeyResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result.Key;
    }

    public async Task<RefreshResponse?> RefreshWalletAsync()
    {
        var request = new ExtendedBaseRequest<RefreshRequest>("refresh");

        var response = await _moneroRPCClient.GetAsync<RefreshResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }

    public async Task<GetTransferByTxIdResponse?> GetTransferByTxIdAsync(string transactionHash)
    {
        var request = new ExtendedBaseRequest<GetTransferByTxIdRequest>("get_transfer_by_txid");
        request.Params.TransactionId = transactionHash;

        var response = await _moneroRPCClient.GetAsync<GetTransferByTxIdResponse>(request);

        if (response is null || response.Result is null)
            return default;

        return response.Result;
    }
}
