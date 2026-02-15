using Wallet.Api.Data.Models;

namespace Wallet.Api.Data.Interfaces;

public interface IWalletDataService
{
    Task<WalletEntity?> CreateWallet(string name);

    Task<bool> RemoveWallet(Guid walletId);

    Task<WalletRowEntity?> AddWalletRow(Guid walletId, string currencyCode, decimal amount);

    Task<bool> RemoveWalletRow(Guid walletRowId);

    Task<WalletEntity?> GetWallet(Guid id);

    Task<IReadOnlyList<WalletEntity>> GetAllWallets();

    Task SaveChanges();
}
