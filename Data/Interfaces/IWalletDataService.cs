using Wallet.Api.DTOs;

namespace Wallet.Api.Data.Interfaces;

public interface IWalletDataService
{
    Task SaveWallet(WalletDto data);
}
