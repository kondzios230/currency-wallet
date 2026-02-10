using Wallet.Api.DTOs;

namespace Wallet.Api.Data;

public interface IExchangeRatesDataService
{
    Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates();
}
