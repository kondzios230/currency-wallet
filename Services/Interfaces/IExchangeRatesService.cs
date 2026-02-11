using Wallet.Api.DTOs;

namespace Wallet.Api.Services.Interfaces;

public interface IExchangeRatesService
{
    Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates();

    Task<IReadOnlyList<ExchangeRateDto>> RefreshExchangeRates();
}
