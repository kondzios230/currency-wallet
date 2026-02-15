using Wallet.Api.Data.Interfaces;
using Wallet.Api.Data.Models;
using Wallet.Api.Services.Interfaces;
using Wallet.Api.Services.Models;

namespace Wallet.Api.Services;

public class ExchangeRatesService : IExchangeRatesService
{
    private readonly IExchangeRatesDataService _exchangeRateDataService;
    private readonly ExchangeRatesAccessLock _accessLock;

    public ExchangeRatesService(
        IExchangeRatesDataService exchangeRateDataService,
        ExchangeRatesAccessLock accessLock)
    {
        _exchangeRateDataService = exchangeRateDataService;
        _accessLock = accessLock;
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> RefreshExchangeRates(CancellationToken cancellationToken = default)
    {
        var handle = await _accessLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await RefreshExchangeRatesCore(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            handle.Dispose();
        }
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates(CancellationToken cancellationToken = default)
    {
        var handle = await _accessLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var dbRates = await _exchangeRateDataService.GetExchangeRatesFromDB(cancellationToken);
            if (!dbRates.Any())
                return await RefreshExchangeRatesCore(cancellationToken).ConfigureAwait(false);

            return ConvertToDto(dbRates);
        }
        finally
        {
            handle.Dispose();
        }
    }

    public async Task<bool> DoesCurrencyExists(string currencyCode, CancellationToken cancellationToken = default)
    {
        var handle = await _accessLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await _exchangeRateDataService.DoesCurrencyExists(currencyCode, cancellationToken);
        }
        finally
        {
            handle.Dispose();
        }
    }

    public async Task<decimal?> GetExchangeRate(string currencyCode, CancellationToken cancellationToken = default)
    {
        var handle = await _accessLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (string.Equals(currencyCode, "PLN", StringComparison.OrdinalIgnoreCase))
                return 1;
            return await _exchangeRateDataService.GetExchangeRateFromDb(currencyCode, cancellationToken);
        }
        finally
        {
            handle.Dispose();
        }
    }

    private async Task<IReadOnlyList<ExchangeRateDto>> RefreshExchangeRatesCore(CancellationToken cancellationToken)
    {
        var rates = await _exchangeRateDataService.GetExchangeRatesFromNbp(cancellationToken);
        await _exchangeRateDataService.SaveExchangeRatesAsync(rates, cancellationToken);
        return ConvertToDto(rates);
    }

    private List<ExchangeRateDto> ConvertToDto(IReadOnlyList<ExchangeRateEntity> rates)
    {
        var list = new List<ExchangeRateDto>();
        foreach (var item in rates)
        {
            list.Add(new ExchangeRateDto { CurrencyCode = item.CurrencyCode, ExchangeRate = item.Rate });
        }

        return list;
    }
}
