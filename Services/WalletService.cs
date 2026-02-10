using Wallet.Api.Data;
using Wallet.Api.DTOs;

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
        return await _walletDataService.CreateWallet(data);
    }
}
