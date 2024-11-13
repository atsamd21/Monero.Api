using System.Text.Json.Serialization;

namespace Monero.Wallet.Responses;

public class MajorMinor
{
    [JsonPropertyName("major")]
    public uint Major { get; set; }
    [JsonPropertyName("minor")]
    public uint Minor { get; set; }
}

public class Transfer
{
    [JsonPropertyName("amount")]
    public ulong Amount { get; set; }
    [JsonPropertyName("amounts")]
    public ulong[] Amounts { get; set; } = [];
    [JsonPropertyName("confirmations")]
    public uint Confirmations { get; set; }
    [JsonPropertyName("double_spend_seen")]
    public bool DoubleSpendSeen { get; set; }
    [JsonPropertyName("height")]
    public uint Height { get; set; }
    [JsonPropertyName("payment_id")]
    public string PaymendId { get; set; } = string.Empty;
    [JsonPropertyName("subaddr_index")]
    public MajorMinor SubAddressIndex { get; set; } = new();
    [JsonPropertyName("subaddr_indices")]
    public MajorMinor[] SubAddressIndices { get; set; } = [];
    [JsonPropertyName("unlock_time")]
    public uint UnlockTime { get; set; }
    [JsonPropertyName("locked")]
    public bool Locked { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class GetTransfersResponse
{
    [JsonPropertyName("in")]
    public List<Transfer> IncomingTransfers { get; set; } = [];
    [JsonPropertyName("pending")]
    public List<Transfer> PendingTransfers { get; set; } = [];
}
