using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class RelayTxResponse
{
    [JsonPropertyName("tx_hash")]
    public string TxHash { get; set; } = string.Empty;
}
