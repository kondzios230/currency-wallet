using Wallet.Api.Data.Models;

namespace Wallet.Api.Data.Interfaces;

public interface IWalletDataService
{
    Task<WalletEntity?> CreateWallet(string name);

    Task<bool> RemoveWallet(Guid walletId);

    Task<WalletRowEntity?> AddWalletRow(Guid walletId, string currencyCode, decimal amount);

    Task<bool> RemoveWalletRow(Guid walletRowId);

    Task<bool> WalletExists(Guid id);

    Task<WalletEntity?> GetWallet(Guid id);

    Task<WalletRowEntity?> GetWalletRow(Guid walletId, string currencyCode);

    Task<IReadOnlyList<WalletEntity>> GetAllWallets();

    Task<int> IncrementWalletRowAmount(Guid walletId, string currencyCode, decimal amount);

    Task<int> DecrementWalletRowAmount(Guid walletId, string currencyCode, decimal amount);

    Task SaveChanges();
}
