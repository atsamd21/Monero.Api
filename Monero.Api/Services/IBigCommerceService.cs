using Monero.Api.BigCommerce.Responses;
using Monero.Api.Models;

namespace Monero.Api.Services;

public interface IBigCommerceService
{
    Task<Response<GetOrderResponse>> GetOrderAsync(int orderId);
    Task<Response<GetOrderResponse>> UpdateOrderAsync(OrderUpdate orderUpdate);
}
