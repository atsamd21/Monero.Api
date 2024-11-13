namespace Monero.Wallet.Responses;

public class GetTransferByTxIdResponse
{
    public Transfer Transfer { get; set; } = new();
    public List<Transfer> Transfers { get; set; } = [];
}
