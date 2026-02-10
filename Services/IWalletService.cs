using Wallet.Api.DTOs;

namespace Wallet.Api.Services;

public interface IWalletService
{
    Task<WalletDto> CreateWallet(WalletDto data);
}
