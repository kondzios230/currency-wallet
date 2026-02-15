using Wallet.Api.Services.Models;

namespace Wallet.Api.Services.Interfaces;

public interface IExchangeRatesService
{
    Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExchangeRateDto>> RefreshExchangeRates(CancellationToken cancellationToken = default);

    Task<bool> DoesCurrencyExists(string currencyCode, CancellationToken cancellationToken = default);

    Task<decimal?> GetExchangeRate(string currencyCode, CancellationToken cancellationToken = default);
}
