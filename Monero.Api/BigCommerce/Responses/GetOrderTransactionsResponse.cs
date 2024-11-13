using System.Text.Json.Serialization;

namespace Monero.Api.BigCommerce.Responses;

public class GetOrderTransactionsResponse
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}
