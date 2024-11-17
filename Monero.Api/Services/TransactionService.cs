using Monero.Api.Data;
using Monero.Wallet;

namespace Monero.Api.Services;

public class TransactionService : ITransactionService
{
    private readonly MoneroApiContext _db;
    private readonly IMoneroWalletService _moneroWalletService;

    public TransactionService(MoneroApiContext db, IMoneroWalletService moneroWalletService)
    {
        _db = db;
        _moneroWalletService = moneroWalletService;
    }
}
