using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class TransferResponse
{
    [JsonPropertyName("amount")]
    public ulong Amount { get; set; }
    [JsonPropertyName("fee")]
    public ulong Fee { get; set; }
    [JsonPropertyName("tx_hash")]
    public string TxHash { get; set; } = string.Empty;
    [JsonPropertyName("tx_metadata")]
    public string TxMetadata { get; set; } = string.Empty;
}
