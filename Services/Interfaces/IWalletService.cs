using Wallet.Api.DTOs;

namespace Wallet.Api.Services.Interfaces;

public interface IWalletService
{
    Task<WalletDto> CreateWallet(WalletDto data);
}
