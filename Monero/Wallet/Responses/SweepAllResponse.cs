using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class SweepAllResponse
{
    [JsonPropertyName("amount")]
    public ulong Amount { get; set; }
    [JsonPropertyName("fee")]
    public ulong Fee { get; set; }
    [JsonPropertyName("tx_hash_list")]
    public string[] TxHashes { get; set; } = [];
    [JsonPropertyName("tx_key_list")]
    public string[] TxKeys { get; set; } = [];
    [JsonPropertyName("fee_list")]
    public ulong[] Fees { get; set; } = [];
    [JsonPropertyName("amount_list")]
    public ulong[] Amounts { get; set; } = [];
}
