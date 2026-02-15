using Wallet.Api.Data.Models;

namespace Wallet.Api.Data.Interfaces;

public interface IWalletDataService
{
    Task<WalletEntity?> CreateWallet(string name, CancellationToken cancellationToken = default);

    Task<bool> RemoveWallet(Guid walletId, CancellationToken cancellationToken = default);

    Task<WalletRowEntity?> AddWalletRow(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default);

    Task<bool> RemoveWalletRow(Guid walletRowId, CancellationToken cancellationToken = default);

    Task<bool> WalletExists(Guid id, CancellationToken cancellationToken = default);

    Task<WalletEntity?> GetWallet(Guid id, CancellationToken cancellationToken = default);

    Task<WalletRowEntity?> GetWalletRow(Guid walletId, string currencyCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WalletEntity>> GetAllWallets(CancellationToken cancellationToken = default);

    Task<int> IncrementWalletRowAmount(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default);

    Task<int> DecrementWalletRowAmount(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default);

    Task Exchange(Guid walletId, string sourceCurrencyCode, string targetCurrencyCode, decimal sourceAmount, decimal targetAmount, CancellationToken cancellationToken = default);
}
