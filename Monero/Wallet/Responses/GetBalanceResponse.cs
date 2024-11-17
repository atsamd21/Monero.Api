using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class SubAddress
{
    [JsonPropertyName("account_index")]
    public uint AccountIndex { get; set; }
    [JsonPropertyName("address_index")]
    public uint AddressIndex { get; set; }
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
    [JsonPropertyName("balance")]
    public ulong Balance { get; set; }
    [JsonPropertyName("unlocked_balance")]
    public ulong UnlockedBalance { get; set; }
    [JsonPropertyName("time_to_unlock")]
    public uint TimeToUnlock { get; set; }
    [JsonPropertyName("blocks_to_unlock")]
    public uint BlocksToUnlock { get; set; }
}

public class GetBalanceResponse
{
    [JsonPropertyName("balance")]
    public ulong Balance { get; set; }
    [JsonPropertyName("unlocked_balance")]
    public ulong UnlockedBalance { get; set; }
    [JsonPropertyName("per_subaddress")]
    public SubAddress[] SubAddresses { get; set; } = [];
}
