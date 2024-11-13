using System.Text.Json.Serialization;

namespace Monero.Api.BigCommerce.Requests;

public class UpdateOrderRequest
{
    [JsonPropertyName("status_id")]
    public int StatusId { get; set; }
}
