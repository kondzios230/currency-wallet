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

    public async Task<IReadOnlyList<ExchangeRateDto>> RefreshExchangeRates()
    {
        var handle = await _accessLock.WaitAsync().ConfigureAwait(false);
        try
        {
            return await RefreshExchangeRatesCore().ConfigureAwait(false);
        }
        finally
        {
            handle.Dispose();
        }
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates()
    {
        var handle = await _accessLock.WaitAsync().ConfigureAwait(false);
        try
        {
            var dbRates = await _exchangeRateDataService.GetExchangeRatesFromDB();
            if (!dbRates.Any())
                return await RefreshExchangeRatesCore().ConfigureAwait(false);

            return ConvertToDto(dbRates);
        }
        finally
        {
            handle.Dispose();
        }
    }

    public async Task<bool> DoesCurrencyExists(string currencyCode)
    {
        var handle = await _accessLock.WaitAsync().ConfigureAwait(false);
        try
        {
            return await _exchangeRateDataService.DoesCurrencyExists(currencyCode);
        }
        finally
        {
            handle.Dispose();
        }
    }

    public async Task<decimal?> GetExchangeRate(string currencyCode)
    {
        var handle = await _accessLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (string.Equals(currencyCode, "PLN", StringComparison.OrdinalIgnoreCase))
                return 1;
            return await _exchangeRateDataService.GetExchangeRateFromDb(currencyCode);
        }
        finally
        {
            handle.Dispose();
        }
    }

    private async Task<IReadOnlyList<ExchangeRateDto>> RefreshExchangeRatesCore()
    {
        var rates = await _exchangeRateDataService.GetExchangeRatesFromNbp();
        await _exchangeRateDataService.SaveExchangeRatesAsync(rates);
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
