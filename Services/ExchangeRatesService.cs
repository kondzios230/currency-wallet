using Wallet.Api.Data.Interfaces;
using Wallet.Api.Data.Models;
using Wallet.Api.Services.Interfaces;
using Wallet.Api.Services.Models;

namespace Wallet.Api.Services;

public class ExchangeRatesService : IExchangeRatesService
{
    private readonly IExchangeRatesDataService _exchangeRateDataService;

    public ExchangeRatesService(
        IExchangeRatesDataService exchangeRateDataService)
    {
        _exchangeRateDataService = exchangeRateDataService;
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> RefreshExchangeRates()
    {
        var rates = await _exchangeRateDataService.GetExchangeRatesFromNbp();
        await _exchangeRateDataService.SaveExchangeRatesAsync(rates);

        return ConvertToDto(rates);
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates()
    {
        var dbRates = await _exchangeRateDataService.GetExchangeRatesFromDB();
        if (!dbRates.Any())
        {
            return await RefreshExchangeRates();
        }

        return ConvertToDto(dbRates);
    }

    public async Task<bool> DoesCurrencyExists(string currencyCode)
    {
        return await _exchangeRateDataService.DoesCurrencyExists(currencyCode);
    }

    public async Task<decimal?> GetExchangeRate(string currencyCode)
    {
        if (string.Equals(currencyCode, "PLN", StringComparison.OrdinalIgnoreCase))
            return 1;

        return await _exchangeRateDataService.GetExchangeRateFromDb(currencyCode);
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
