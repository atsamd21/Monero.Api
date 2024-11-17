using Monero.Wallet.Responses;

namespace Monero.Wallet;

public interface IMoneroWalletService
{
    Task<CreateAddressResponse?> CreateAddressAsync(uint accountIndex);
    Task<CreateAccountResponse?> CreateAccountAsync();
    Task<GetAccountsResponse?> GetAccountsAsync();
    Task<GetBalanceResponse?> GetBalanceAsync(uint accountIndex, uint[] indices);
    Task<GetAddressResponse?> GetAddressAsync();
    Task<EstimateTxSizeAndWeightResponse?> GetSizeAndWeightEstimateAsync();
    Task<TransferResponse?> TransferAsync(ulong amount, uint fromAccountIndex, string toAddress, uint[] subbaddressIndices, bool subtractFee);
    Task<RelayTxResponse?> RelayTxAsync(string hex);
    Task<GetFeeResponse?> GetRealFeeAsync(ulong amount, uint fromAccount, string toAddress, uint[] subbaddressIndices);
    Task<SweepAllResponse?> SweepAllAsync(uint fromAccount, string toAddress);
    Task<RefreshResponse?> RefreshWalletAsync();
    Task<GetTransferByTxIdResponse?> GetTransferByTxIdAsync(string transactionHash);
    Task<GetTransfersResponse?> GetPendingAndIncomingTransfersAsync(uint accountIndex, uint[] subAddressIndices);
    Task<string?> PayFromMasterAsync(ulong amount, string address);
    Task<string?> GetSpendKeyAsync();
    Task<bool> CreateWalletAsync(string walletName);
    Task<bool> OpenWalletAsync(string walletName);
    Task<bool> SaveWalletAsync();
    Task SetDaemonAsync();
    bool CheckIfWalletExists(string walletName);
    bool StartWalletRPC();
}