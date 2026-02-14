using Wallet.Api.Services.Models;

namespace Wallet.Api.Services.Interfaces;

public interface IExchangeRatesService
{
    Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates();

    Task<IReadOnlyList<ExchangeRateDto>> RefreshExchangeRates();

    Task<bool> DoesCurrencyExists(string currencyCode);
}
