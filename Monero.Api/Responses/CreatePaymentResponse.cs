using Monero.Api.Models;

namespace Monero.Api.Responses;

public class CreatePaymentResponse
{
    public Guid PaymentId { get; set; }
    public decimal XMRAmount { get; set; }
    public string Address { get; set; } = string.Empty;
    public PaymentState PaymentState { get; set; }
}
