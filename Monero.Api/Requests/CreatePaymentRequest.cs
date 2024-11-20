namespace Monero.Api.Requests;

public class CreatePaymentRequest
{
    public string Email { get; set; } = string.Empty;
    public int OrderId { get; set; }
}
