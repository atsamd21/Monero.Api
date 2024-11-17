using System.Text.Json.Serialization;

namespace Monero.Daemon.Responses;

public class FeeEstimateResponse
{
    [JsonPropertyName("credits")]
    public int Credits { get; set; }
    [JsonPropertyName("fee")]
    public int Fee { get; set; }
    [JsonPropertyName("fees")]
    public int[] Fees { get; set; } = [];
    [JsonPropertyName("quantization_mask")]
    public int QuantizationMask { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    [JsonPropertyName("top_hash")]
    public string TopHash { get; set; } = string.Empty;
    [JsonPropertyName("untrusted")]
    public bool Untrusted { get; set; }
}
