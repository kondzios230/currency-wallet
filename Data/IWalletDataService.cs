using Wallet.Api.DTOs;

namespace Wallet.Api.Data;

public interface IWalletDataService
{
    Task<WalletDto> CreateWallet(WalletDto data);
}
