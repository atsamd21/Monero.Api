using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class CreateWalletRequest
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;
    [JsonPropertyName("language")]
    public string Language { get; set; } = "English";
    [JsonPropertyName("password ")]
    public string Password { get; set; } = string.Empty;
}
