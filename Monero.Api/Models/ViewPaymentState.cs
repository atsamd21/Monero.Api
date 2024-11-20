namespace Monero.Api.Models;

public enum ViewPaymentState
{
    Unpaid,
    Pending,
    Paid,
    Expired,
    Archived
}

public static class ViewPaymentStateExtensions
{
    public static ViewPaymentState ToViewPaymentState(this PaymentState paymentState)
    {
        return paymentState switch
        {
            PaymentState.Unpaid => ViewPaymentState.Unpaid,
            PaymentState.Pending => ViewPaymentState.Pending,
            PaymentState.Paid => ViewPaymentState.Paid,
            PaymentState.Withdrawn => ViewPaymentState.Paid,
            PaymentState.Expired => ViewPaymentState.Expired,
            _ => ViewPaymentState.Archived,
        };
    }
}
