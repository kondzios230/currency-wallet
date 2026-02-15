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

    public async Task<WalletDto> CreateWallet(string walletName, CancellationToken cancellationToken = default)
    {
        var entity = await _walletDataService.CreateWallet(walletName, cancellationToken);
        if (entity == null)
            throw new InvalidOperationException("A wallet with this name already exists.");

        return ConvertWalletToDto(entity);
    }

    public async Task<WalletDto?> GetWallet(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _walletDataService.GetWallet(id, cancellationToken);
        return entity == null ? null : ConvertWalletToDto(entity);
    }

    public async Task<IReadOnlyList<WalletDto>> GetAllWallets(CancellationToken cancellationToken = default)
    {
        var entities = await _walletDataService.GetAllWallets(cancellationToken);
        return entities.Select(ConvertWalletToDto).ToList();
    }

    public async Task RemoveWallet(Guid walletId, CancellationToken cancellationToken = default)
    {
        var removed = await _walletDataService.RemoveWallet(walletId, cancellationToken);
        if (!removed)
            throw new InvalidOperationException("Wallet not found.");
    }

    public async Task<WalletRowDto> TopUpWallet(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default)
    {
        var roundedAmount = Math.Round(amount, 2);

        if (!await _walletDataService.WalletExists(walletId, cancellationToken))
            throw new InvalidOperationException("Wallet not found.");

        var rowsUpdated = await _walletDataService.IncrementWalletRowAmount(walletId, currencyCode, roundedAmount, cancellationToken);
        if (rowsUpdated == 0)
            await _walletDataService.AddWalletRow(walletId, currencyCode, roundedAmount, cancellationToken);

        var row = await _walletDataService.GetWalletRow(walletId, currencyCode, cancellationToken);
        if (row == null)
            throw new InvalidOperationException("Wallet row could not be read after update.");

        return ConvertWalletRowToDto(row);
    }

    public async Task<WalletRowDto?> WithdrawFromWallet(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default)
    {
        var roundedAmount = Math.Round(amount, 2);

        if (!await _walletDataService.WalletExists(walletId, cancellationToken))
            throw new InvalidOperationException("Wallet not found.");

        var rowsUpdated = await _walletDataService.DecrementWalletRowAmount(walletId, currencyCode, roundedAmount, cancellationToken);
        if (rowsUpdated == 0)
            throw new InvalidOperationException("Currency does not exist in this wallet or insufficient balance.");

        var row = await _walletDataService.GetWalletRow(walletId, currencyCode, cancellationToken);
        if (row == null)
            throw new InvalidOperationException("Wallet row could not be read after withdraw.");

        if (row.Amount == 0)
        {
            await _walletDataService.RemoveWalletRow(row.Id, cancellationToken);
            return null;
        }

        return ConvertWalletRowToDto(row);
    }

    public async Task ExchangeAsync(Guid walletId, string sourceCurrencyCode, string targetCurrencyCode, decimal sourceAmount, CancellationToken cancellationToken = default)
    {
        if (!await _walletDataService.WalletExists(walletId, cancellationToken))
            throw new InvalidOperationException("Wallet not found.");

        if (string.Equals(sourceCurrencyCode, targetCurrencyCode, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Source and target currency must differ.");
        if (!await _exchangeRatesService.DoesCurrencyExists(sourceCurrencyCode, cancellationToken))
            throw new InvalidOperationException("Source currency code is invalid.");
        if (!await _exchangeRatesService.DoesCurrencyExists(targetCurrencyCode, cancellationToken))
            throw new InvalidOperationException("Target currency code is invalid.");

        var rateSource = await _exchangeRatesService.GetExchangeRate(sourceCurrencyCode, cancellationToken);
        var rateTarget = await _exchangeRatesService.GetExchangeRate(targetCurrencyCode, cancellationToken);
        if (rateSource == null)
            throw new InvalidOperationException("Exchange rate not available for source currency.");
        if (rateTarget == null)
            throw new InvalidOperationException("Exchange rate not available for target currency.");

        var roundedSourceAmount = Math.Round(sourceAmount, 2);
        var plnAmount = roundedSourceAmount * rateSource.Value;
        var targetAmount = Math.Round(plnAmount / rateTarget.Value, 2);

        await _walletDataService.Exchange(walletId, sourceCurrencyCode, targetCurrencyCode, roundedSourceAmount, targetAmount, cancellationToken);
    }

    private WalletDto ConvertWalletToDto(WalletEntity entity)
    {
        return new WalletDto
        {
            Id = entity.Id,
            WalletName = entity.Name,
            Rows = entity.Rows.Select(r => ConvertWalletRowToDto(r)).ToList()
        };
    }

    private WalletRowDto ConvertWalletRowToDto(WalletRowEntity entity)
    {
        return new WalletRowDto
        {
            Id = entity.Id,
            WalletId = entity.WalletId,
            CurrencyCode = entity.CurrencyCode,
            Amount = entity.Amount
        };
    }
}
