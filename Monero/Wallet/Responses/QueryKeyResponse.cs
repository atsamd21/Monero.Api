using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class QueryKeyResponse
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
}
