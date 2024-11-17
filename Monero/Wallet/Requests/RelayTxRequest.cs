using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class RelayTxRequest
{
    [JsonPropertyName("hex")]
    public string Hex { get; set; } = string.Empty;
}
