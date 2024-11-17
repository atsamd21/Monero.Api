using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class SubAddressAccount
{
    [JsonPropertyName("account_index")]
    public uint AccountIndex { get; set; }
    [JsonPropertyName("base_address")]
    public string Address { get; set; } = string.Empty;
    [JsonPropertyName("balance")]
    public ulong Balance { get; set; }
    [JsonPropertyName("unlocked_balance")]
    public ulong UnlockedBalance { get; set; }
}

public class GetAccountsResponse
{
    [JsonPropertyName("subaddress_accounts")]
    public SubAddressAccount[] SubAddressAccounts { get; set; } = [];
}
