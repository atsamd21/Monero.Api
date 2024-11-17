using Microsoft.EntityFrameworkCore;
using Monero.Api.Data;
using Monero.Api.Models;
using Monero.Api.Requests;
using Monero.Api.Responses;
using Monero.Helpers;
using Monero.Singletons;
using Monero.Wallet;

namespace Monero.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly IMoneroWalletService _moneroWalletService;
    private readonly IBigCommerceService _bigCommerceService;
    private readonly MoneroPriceSingleton _moneroPriceSingleton;
    private readonly MoneroApiContext _db;

    public PaymentService(IMoneroWalletService moneroWalletService, MoneroPriceSingleton moneroPriceSingleton, IBigCommerceService bigCommerceService, MoneroApiContext db)
    {
        _moneroWalletService = moneroWalletService;
        _moneroPriceSingleton = moneroPriceSingleton;
        _bigCommerceService = bigCommerceService;
        _db = db;
    }

    public async Task<Response<CreatePaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request)
    {
        Response<CreatePaymentResponse> response = new();

        var existingPayment = await _db.Payments.FirstOrDefaultAsync(x => x.OrderId == request.OrderId);
        if (existingPayment is not null)
        {
            response.Value = new()
            {
                Address = existingPayment.Address,
                PaymentId = existingPayment.Id,
                XMRAmount = existingPayment.XMRAmount,
                PaymentState = existingPayment.PaymentState,
            };

            return response;
        }

        var orderResponse = await _bigCommerceService.GetOrderAsync(request.OrderId);
        if (orderResponse.Value is null)
        {
            response.Message = orderResponse.Message;
            return response;
        }

        var createAddressResponse = await _moneroWalletService.CreateAddressAsync(0);

        if (createAddressResponse is null)
        {
            response.Message = "Error.";
            return response;
        }

        var amount = orderResponse.Value.SubTotal + 
            orderResponse.Value.WrappingCost + 
            orderResponse.Value.HandlingCost + 
            orderResponse.Value.ShippingCost - 
            orderResponse.Value.CouponDiscount -
            orderResponse.Value.DiscountAmount;

        decimal xmrAmount = _moneroPriceSingleton.Prices[orderResponse.Value.CurrencyCode] * amount;
        xmrAmount = Math.Round(xmrAmount, 12);

        var utcNow = DateTime.UtcNow;

        var payment = new Payment
        {
            Id = new Guid(),
            OrderId = request.OrderId,
            AddressIndex = createAddressResponse.AddressIndex,
            Address = createAddressResponse.Address,
            CreatedAt = utcNow,
            UpdatedAt = utcNow,
            FiatAmount = amount,
            Currency = orderResponse.Value.CurrencyCode,
            Email = orderResponse.Value.BillingAddress.Email,
            Piconero = xmrAmount.MoneroToPiconero(),
            XMRAmount = xmrAmount,
            ValidUntil = utcNow.AddDays(1),
            CustomerId = orderResponse.Value.CustomerId
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        response.Value = new()
        {
            Address = createAddressResponse.Address,
            PaymentId = payment.Id,
            XMRAmount = xmrAmount
        };

        return response;
    }
}
