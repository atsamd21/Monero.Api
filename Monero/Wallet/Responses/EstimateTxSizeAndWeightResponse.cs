using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class EstimateTxSizeAndWeightResponse
{
    [JsonPropertyName("size")]
    public int Size { get; set; }
    [JsonPropertyName("weight")]
    public int Weight { get; set; }
}
