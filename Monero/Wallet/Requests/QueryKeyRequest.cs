using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class QueryKeyRequest
{
    [JsonPropertyName("key_type")]
    public string KeyType { get; set; } = "spend_key";
}
