using Wallet.Api.Services.Models;

namespace Wallet.Api.Services.Interfaces;

public interface IWalletService
{
    Task<WalletDto> CreateWallet(string walletName);

    Task<WalletDto?> GetWallet(Guid id);

    Task<IReadOnlyList<WalletDto>> GetAllWallets();

    Task RemoveWallet(Guid walletId);

    Task<WalletRowDto> TopUpWallet(Guid walletId, string currencyCode, decimal amount);

    Task<WalletRowDto?> WithdrawFromWallet(Guid walletId, string currencyCode, decimal amount);

    Task ExchangeAsync(Guid walletId, string sourceCurrencyCode, string targetCurrencyCode, decimal sourceAmount);
}
