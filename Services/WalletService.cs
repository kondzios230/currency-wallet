using Wallet.Api.Data.Interfaces;
using Wallet.Api.DTOs;
using Wallet.Api.Services.Interfaces;

namespace Wallet.Api.Services;

public class WalletService : IWalletService
{
    private readonly IWalletDataService _walletDataService;

    public WalletService(IWalletDataService walletDataService)
    {
        _walletDataService = walletDataService;
    }

    public async Task<WalletDto> CreateWallet(WalletDto data)
    {
        throw new Exception();
    }
}
