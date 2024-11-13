using Microsoft.AspNetCore.Mvc;
using Monero.Api.Requests;
using Monero.Api.Responses;
using Monero.Api.Services;

namespace Monero.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class MoneroPaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public MoneroPaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<ActionResult<CreatePaymentResponse>> PostPaymentAsync(CreatePaymentRequest request)
    {
        var response = await _paymentService.CreatePaymentAsync(request);

        if (response.Value is null)
            return StatusCode(StatusCodes.Status500InternalServerError, response.Message);

        return Ok(response.Value);
    }
}
