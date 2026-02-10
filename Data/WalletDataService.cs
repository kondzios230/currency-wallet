using Wallet.Api.DTOs;

namespace Wallet.Api.Data;

public sealed class WalletDataService : IWalletDataService
{
    private readonly Dictionary<string, WalletDto> _wallets = new();

    public Task<WalletDto> CreateWallet(WalletDto data)
    {
        if (_wallets.ContainsKey(data.WalletName))
        {
            throw new InvalidOperationException($"Wallet with name {data.WalletName} already exists.");
        }

        _wallets[data.WalletName] = data;
        return Task.FromResult(data);
    }
}
