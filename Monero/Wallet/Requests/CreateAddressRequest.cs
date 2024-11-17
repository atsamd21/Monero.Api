using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class CreateAddressRequest
{
    [JsonPropertyName("account_index")]
    public uint AccountIndex { get; set; }
}
