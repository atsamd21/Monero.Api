using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class EstimateTxSizeAndWeightRequest
{
    [JsonPropertyName("n_inputs")]
    public uint NInputs { get; set; } = 1;
    [JsonPropertyName("n_outputs")]
    public uint NOutputs { get; set; } = 2;
    [JsonPropertyName("ring_size")]
    public uint RingSize { get; set; } = 16;
    [JsonPropertyName("rct")]
    public bool RCT { get; set; } = true;
}
