using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class RefreshRequest
{
    [JsonPropertyName("start_height")]
    public uint StartHeight { get; set; }
}
