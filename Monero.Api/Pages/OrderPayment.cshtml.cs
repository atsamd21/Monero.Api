using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Monero.Api.Requests;
using Monero.Api.Responses;
using Monero.Api.Services;

namespace Monero.Api.Pages;

public class OrderPayment : PageModel
{
    private readonly IPaymentService _paymentService;
    public CreatePaymentResponse CreatePaymentResponse { get; set; } = default!;
    public string Email { get; set; } = string.Empty;
    public string Store { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public bool IsEmbedded { get; set; }

    public OrderPayment(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<ActionResult> OnGetAsync(string email, int orderId, string store, bool? isEmbedded)
    {
        var response = await _paymentService.CreatePaymentAsync(new CreatePaymentRequest { Email = email, OrderId = orderId });
        if (response.Value is null)
            return NotFound();

        if (isEmbedded is not null)
        {
            IsEmbedded = isEmbedded.Value;
        }

        CreatePaymentResponse = response.Value;
        Email = email;
        OrderId = orderId;
        Store = store;

        return Page();
    }
}