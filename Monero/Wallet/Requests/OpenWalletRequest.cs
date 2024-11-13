using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class OpenWalletRequest
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;
    [JsonPropertyName("password ")]
    public string Password { get; set; } = string.Empty;
}
