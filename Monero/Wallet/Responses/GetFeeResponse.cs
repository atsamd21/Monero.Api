namespace Monero.Wallet.Responses;

public class GetFeeResponse
{
    public ulong Fee { get; set; }
    public string TxMetadata { get; set; } = string.Empty;
}

