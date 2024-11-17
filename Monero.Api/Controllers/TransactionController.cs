using Microsoft.AspNetCore.Mvc;
using Monero.Api.Attributes;
using Monero.Api.Singletons;

namespace Monero.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class TransactionController : ControllerBase
{
    private readonly PaymentSingleton _paymentSingleton;

    public TransactionController(PaymentSingleton paymentSingleton)
    {
        _paymentSingleton = paymentSingleton;
    }

    [HttpGet]
    [ApiKeyAuthorize]
    public async Task<ActionResult> TransactionNotify(string transactionHash)
    {
        await _paymentSingleton.QueueTransactionAsync(transactionHash);

        return Ok();
    }
}
