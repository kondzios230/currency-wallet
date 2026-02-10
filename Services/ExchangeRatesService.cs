using Wallet.Api.Data;
using Wallet.Api.DTOs;

namespace Wallet.Api.Services;

public class ExchangeRatesService : IExchangeRatesService
{
    private readonly IExchangeRatesDataService _exchangeRateDataService;
    public ExchangeRatesService(IExchangeRatesDataService exchangeRateDataService)
    {
        _exchangeRateDataService = exchangeRateDataService;
    }

    public Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates()
    {
        return _exchangeRateDataService.GetExchangeRates();
    }
}
