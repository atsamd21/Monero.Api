using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class CreateAddressResponse
{
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
    [JsonPropertyName("address_index")]
    public uint AddressIndex { get; set; }
    [JsonPropertyName("address_indices")]
    public uint[] AddressIndices { get; set; } = [];
    [JsonPropertyName("addresses")]
    public string[] Addresses { get; set; } = [];
}
