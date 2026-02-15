using Wallet.Api.Data.Interfaces;
using Wallet.Api.Data.Models;
using Wallet.Api.Services.Interfaces;
using Wallet.Api.Services.Models;

namespace Wallet.Api.Services;

public class WalletService : IWalletService
{
    private readonly IWalletDataService _walletDataService;
    private readonly IExchangeRatesService _exchangeRatesService;

    public WalletService(IWalletDataService walletDataService, IExchangeRatesService exchangeRatesService)
    {
        _walletDataService = walletDataService;
        _exchangeRatesService = exchangeRatesService;
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

    public async Task ExchangeAsync(Guid walletId, string sourceCurrencyCode, string targetCurrencyCode, decimal sourceAmount)
    {
        var wallet = await _walletDataService.GetWallet(walletId);
        if (wallet == null)
            throw new InvalidOperationException("Wallet not found.");

        if (string.Equals(sourceCurrencyCode, targetCurrencyCode, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Source and target currency must differ.");
        if (!await _exchangeRatesService.DoesCurrencyExists(sourceCurrencyCode))
            throw new InvalidOperationException("Source currency code is invalid.");
        if (!await _exchangeRatesService.DoesCurrencyExists(targetCurrencyCode))
            throw new InvalidOperationException("Target currency code is invalid.");

        var sourceRow = wallet.Rows.FirstOrDefault(r => r.CurrencyCode == sourceCurrencyCode);
        if (sourceRow == null)
            throw new InvalidOperationException("Source currency does not exist in this wallet.");

        var rateSource = await _exchangeRatesService.GetExchangeRate(sourceCurrencyCode);
        var rateTarget = await _exchangeRatesService.GetExchangeRate(targetCurrencyCode);
        if (rateSource == null)
            throw new InvalidOperationException("Exchange rate not available for source currency.");
        if (rateTarget == null)
            throw new InvalidOperationException("Exchange rate not available for target currency.");

        var roundedAmount = Math.Round(sourceAmount, 2);
        if (sourceRow.Amount < roundedAmount)
            throw new InvalidOperationException("Insufficient balance. Amount cannot exceed the current wallet balance for the source currency.");

        var plnAmount = roundedAmount * rateSource.Value;
        var targetAmount = Math.Round(plnAmount / rateTarget.Value, 2);

        sourceRow.Amount -= roundedAmount;
        if (sourceRow.Amount == 0)
            wallet.Rows.Remove(sourceRow);

        var targetRow = wallet.Rows.FirstOrDefault(r => r.CurrencyCode == targetCurrencyCode);
        if (targetRow != null)
            targetRow.Amount += targetAmount;
        else
        {
            wallet.Rows.Add(new WalletRowEntity
            {
                Id = Guid.NewGuid(),
                WalletId = walletId,
                CurrencyCode = targetCurrencyCode,
                Amount = targetAmount
            });
        }

        await _walletDataService.SaveChanges();
    }
}
