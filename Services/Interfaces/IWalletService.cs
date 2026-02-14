using Wallet.Api.Services.Models;

namespace Wallet.Api.Services.Interfaces;

public interface IWalletService
{
    Task<WalletDto> CreateWallet(string walletName);

    Task RemoveWallet(Guid walletId);

    Task<WalletRowDto> TopUpWallet(Guid walletId, string currencyCode, decimal amount);

    Task<WalletRowDto?> WithdrawFromWallet(Guid walletId, string currencyCode, decimal amount);
}
