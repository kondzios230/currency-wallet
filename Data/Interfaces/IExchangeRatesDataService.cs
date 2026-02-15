using Wallet.Api.Data.Models;

namespace Wallet.Api.Data.Interfaces;

public interface IExchangeRatesDataService
{
    Task<IReadOnlyList<ExchangeRateEntity>> GetExchangeRatesFromDB(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExchangeRateEntity>> GetExchangeRatesFromNbp(CancellationToken cancellationToken = default);

    Task SaveExchangeRatesAsync(IReadOnlyList<ExchangeRateEntity> exchangeRates, CancellationToken cancellationToken = default);

    Task<bool> DoesCurrencyExists(string currencyCode, CancellationToken cancellationToken = default);

    Task<decimal?> GetExchangeRateFromDb(string currencyCode, CancellationToken cancellationToken = default);
}
