using Microsoft.Extensions.Options;
using Monero.Api.BigCommerce.Requests;
using Monero.Api.BigCommerce.Responses;
using Monero.Api.Helpers;
using Monero.Api.Models;

namespace Monero.Api.Services;

public class BigCommerceService : IBigCommerceService
{
    private readonly HttpClient _httpClient;
    private readonly BigCommerceSettings _bigCommerceSettings;

    public BigCommerceService(HttpClient httpClient, IOptions<BigCommerceSettings> options)
    {
        _httpClient = httpClient;
        _bigCommerceSettings = options.Value;
    }

    // Note: orderId is sequential - safe?
    public async Task<Response<GetOrderResponse>> GetOrderAsync(int orderId)
    {
        return await _httpClient.GetFromApiAsync<GetOrderResponse>($"stores/{_bigCommerceSettings.StoreHash}/v2/orders/{orderId}", _bigCommerceSettings.APIKey);
    }

    public async Task<Response<GetOrderResponse>> UpdateOrderAsync(OrderUpdate orderUpdate)
    {
        var request = new UpdateOrderRequest
        {
            StatusId = orderUpdate.StatusId
        };

        return await _httpClient.PutToApiAsync<GetOrderResponse>(request, $"stores/{_bigCommerceSettings.StoreHash}/v2/orders/{orderUpdate.OrderId}", _bigCommerceSettings.APIKey);
    }
}
