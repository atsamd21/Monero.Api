namespace Monero.Api.Models;

public enum PaymentState
{
    Unpaid,
    Pending,
    Paid,
    Withdrawn,
    Expired,
    Deleted
}

public class Payment
{
    public Guid Id { get; set; }
    public long OrderId { get; set; }
    public long CustomerId { get; set; }
    public uint AddressIndex { get; set; }
    public string Address { get; set; } = string.Empty;
    public ulong Piconero { get; set; }
    public decimal XMRAmount { get; set; }
    public decimal FiatAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentState PaymentState { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ValidUntil { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Email { get; set; } = string.Empty;
}