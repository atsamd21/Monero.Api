using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class GetTransfersRequest
{
    [JsonPropertyName("subaddr_indices")]
    public uint[] SubAddressIndices { get; set; } = [];
    [JsonPropertyName("in")]
    public bool In { get; set; }
    [JsonPropertyName("pending")]
    public bool Pending { get; set; }
    [JsonPropertyName("account_index")]
    public uint AccountIndex { get; set; }
}
