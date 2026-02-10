using Wallet.Api.DTOs;

namespace Wallet.Api.Data;

public sealed class ExchangeRateDataService : IExchangeRatesDataService
{
    public Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates()
    {
        throw new NotImplementedException();
    }
}
