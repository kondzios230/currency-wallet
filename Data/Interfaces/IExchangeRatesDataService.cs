using Wallet.Api.Data.Models;

namespace Wallet.Api.Data.Interfaces;

public interface IExchangeRatesDataService
{
    Task<IReadOnlyList<ExchangeRateEntity>> GetExchangeRatesFromDB();

    Task<IReadOnlyList<ExchangeRateEntity>> GetExchangeRatesFromNbp();

    Task SaveExchangeRatesAsync(IReadOnlyList<ExchangeRateEntity> exchangeRates);

    Task<bool> DoesCurrencyExists(string currencyCode);
}
