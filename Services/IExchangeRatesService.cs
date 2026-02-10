using Wallet.Api.DTOs;

namespace Wallet.Api.Services;

public interface IExchangeRatesService
{
    Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates();

}
