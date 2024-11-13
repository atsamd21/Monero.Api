using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class Addresses
{
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
    [JsonPropertyName("address_index")]
    public int AddressIndex { get; set; } = 0;
    [JsonPropertyName("used")]
    public bool Used { get; set; }
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}

public class GetAddressResponse
{
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
    [JsonPropertyName("addresses")]
    public Addresses[] Addresses { get; set; } = [];
}
