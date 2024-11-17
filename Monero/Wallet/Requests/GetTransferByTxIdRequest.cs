using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class GetTransferByTxIdRequest
{
    [JsonPropertyName("txid")]
    public string TransactionId { get; set; } = string.Empty;
    [JsonPropertyName("account_index")]
    public uint AccountIndex { get; set; }
}
