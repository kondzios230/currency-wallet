using Wallet.Api.Data.Interfaces;
using Wallet.Api.DTOs;
using Wallet.Api.Services.Interfaces;

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
        return rates;
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates()
    {
        return await _exchangeRateDataService.GetExchangeRatesFromDB();
    }
}
