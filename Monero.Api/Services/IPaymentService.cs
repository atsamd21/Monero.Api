using Monero.Api.Models;
using Monero.Api.Requests;
using Monero.Api.Responses;

namespace Monero.Api.Services;

public interface IPaymentService
{
    Task<Response<CreatePaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request);
}
