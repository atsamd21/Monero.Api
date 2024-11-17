using System.Text.Json.Serialization;

namespace Monero.Wallet.Requests;

public class Destination
{
    [JsonPropertyName("amount")]
    public ulong Amount { get; set; }
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
}

public class TransferRequest
{
    [JsonPropertyName("destinations")]
    public Destination[] Destinations { get; set; } = [];
    [JsonPropertyName("priority")]
    public uint Priority { get; set; } = 2;
    [JsonPropertyName("unlock_time")]
    public uint UnlockTime { get; set; }
    [JsonPropertyName("mixin")]
    public uint Mixin { get; set; } = 4;
    [JsonPropertyName("ring_size")]
    public uint RingSize { get; set; } = 16;
    [JsonPropertyName("get_tx_hex")]
    public bool GetTxHex { get; set; } = true;
    [JsonPropertyName("get_tx_metadata")]
    public bool GetTxMetadata { get; set; } = true;
    [JsonPropertyName("do_not_relay")]
    public bool DoNotRelay { get; set; }
    [JsonPropertyName("account_index")]
    public uint AccountIndex { get; set; }
    [JsonPropertyName("subaddr_indices")]
    public uint[] SubaddressIndices { get; set; } = [];
}
