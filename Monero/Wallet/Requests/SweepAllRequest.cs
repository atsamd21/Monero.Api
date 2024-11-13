using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class SweepAllRequest
{
    [JsonPropertyName("account_index")]
    public uint AccountIndex { get; set; }
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
    [JsonPropertyName("priority")]
    public uint Priority { get; set; } = 2;
    [JsonPropertyName("unlock_time")]
    public uint UnlockTime { get; set; }
}
