using Wallet.Api.Services.Models;

namespace Wallet.Api.Services.Interfaces;

public interface IWalletService
{
    Task<WalletDto> CreateWallet(string walletName, CancellationToken cancellationToken = default);

    Task<WalletDto?> GetWallet(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WalletDto>> GetAllWallets(CancellationToken cancellationToken = default);

    Task RemoveWallet(Guid walletId, CancellationToken cancellationToken = default);

    Task<WalletRowDto> TopUpWallet(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default);

    Task<WalletRowDto?> WithdrawFromWallet(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default);

    Task ExchangeAsync(Guid walletId, string sourceCurrencyCode, string targetCurrencyCode, decimal sourceAmount, CancellationToken cancellationToken = default);
}
