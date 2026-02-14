using Wallet.Api.Data.Interfaces;
using Wallet.Api.Data.Models;
using Wallet.Api.Services.Interfaces;
using Wallet.Api.Services.Models;

namespace Wallet.Api.Services;

public class WalletService : IWalletService
{
    private readonly IWalletDataService _walletDataService;

    public WalletService(IWalletDataService walletDataService)
    {
        _walletDataService = walletDataService;
    }

    public async Task<WalletDto> CreateWallet(string walletName)
    {
        var entity = await _walletDataService.CreateWallet(walletName);
        if (entity == null)
            throw new InvalidOperationException("A wallet with this name already exists.");

        return new WalletDto
        {
            Id = Guid.NewGuid(),
            WalletName = entity.Name,
            Rows = new List<WalletRowDto>()
        };
    }

    public async Task RemoveWallet(Guid walletId)
    {
        var removed = await _walletDataService.RemoveWallet(walletId);
        if (!removed)
            throw new InvalidOperationException("Wallet not found.");
    }

    public async Task<WalletRowDto> TopUpWallet(Guid walletId, string currencyCode, decimal amount)
    {
        var roundedAmount = Math.Round(amount, 2);

        var wallet = await _walletDataService.GetWallet(walletId);
        if (wallet == null)
            throw new InvalidOperationException("Wallet not found.");

        var existingRow = wallet.Rows.FirstOrDefault(r => r.CurrencyCode == currencyCode);
        if (existingRow != null)
        {
            existingRow.Amount += roundedAmount;
        }
        else
        {
            var newRow = new WalletRowEntity
            {
                Id = Guid.NewGuid(),
                WalletId = walletId,
                CurrencyCode = currencyCode,
                Amount = roundedAmount
            };
            wallet.Rows.Add(newRow);
        }

        await _walletDataService.SaveChanges();

        var updatedRow = wallet.Rows.First(r => r.CurrencyCode == currencyCode);
        return new WalletRowDto
        {
            Id = updatedRow.Id,
            WalletId = updatedRow.WalletId,
            CurrencyCode = updatedRow.CurrencyCode,
            Amount = updatedRow.Amount
        };
    }

    public async Task<WalletRowDto?> WithdrawFromWallet(Guid walletId, string currencyCode, decimal amount)
    {
        var roundedAmount = Math.Round(amount, 2);

        var wallet = await _walletDataService.GetWallet(walletId);
        if (wallet == null)
            throw new InvalidOperationException("Wallet not found.");

        var existingRow = wallet.Rows.FirstOrDefault(r => r.CurrencyCode == currencyCode);
        if (existingRow == null)
            throw new InvalidOperationException("Currency does not exist in this wallet.");

        if (roundedAmount > existingRow.Amount)
            throw new InvalidOperationException("Insufficient balance. Amount cannot exceed the current wallet balance for this currency.");

        existingRow.Amount -= roundedAmount;

        if (existingRow.Amount == 0)
        {
            wallet.Rows.Remove(existingRow);
            await _walletDataService.SaveChanges();
            return null;
        }

        await _walletDataService.SaveChanges();
        return new WalletRowDto
        {
            Id = existingRow.Id,
            WalletId = existingRow.WalletId,
            CurrencyCode = existingRow.CurrencyCode,
            Amount = existingRow.Amount
        };
    }
}
