using Wallet.Api.DTOs;

namespace Wallet.Api.Data.Interfaces;

public interface IExchangeRatesDataService
{
    Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRatesFromDB();

    Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRatesFromNbp();

    Task SaveExchangeRatesAsync(IReadOnlyList<ExchangeRateDto> exchangeRates);
}
