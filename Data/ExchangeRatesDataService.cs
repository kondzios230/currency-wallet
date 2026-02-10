using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Wallet.Api.DTOs;

namespace Wallet.Api.Data;

public sealed class ExchangeRateDataService : IExchangeRatesDataService
{
    private const string HttpClientName = "Nbp";

    private const string TableBUrl = "https://api.nbp.pl/api/exchangerates/tables/B/";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly IHttpClientFactory _httpClientFactory;

    public ExchangeRateDataService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRates()
    {
        var client = _httpClientFactory.CreateClient(HttpClientName);
        try
        {
            var response = await client.GetAsync(TableBUrl);

            var tables = await response.Content.ReadFromJsonAsync<NbpApiResponse[]>(JsonOptions);
            if (tables is null || tables.Length == 0)
            {
                return new List<ExchangeRateDto>();
            }

            var table = tables.First();
            var rates = new List<ExchangeRateDto>();
            foreach (var r in table.Rates)
            {
                rates.Add(new ExchangeRateDto
                {
                    CurrencyCode = r.Code,
                    ExchangeRate = r.Mid
                });
            }

            return rates;
        }
        catch (TaskCanceledException)
        {
            throw new Exception("NBP request timed out.");
        }
        catch (HttpRequestException)
        {
            throw new Exception("NBP HTTP request failed");
        }
    }
}
