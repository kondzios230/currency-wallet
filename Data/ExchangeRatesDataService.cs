using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Wallet.Api.Data.Interfaces;
using Wallet.Api.Data.Models;

namespace Wallet.Api.Data;

public sealed class ExchangeRatesDataService : IExchangeRatesDataService
{
    private const string HttpClientName = "Nbp";

    private const string TableBUrl = "https://api.nbp.pl/api/exchangerates/tables/B/";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _context;

    public ExchangeRatesDataService(IHttpClientFactory httpClientFactory, AppDbContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    public async Task<IReadOnlyList<ExchangeRateEntity>> GetExchangeRatesFromDB()
    {
        var entities = await _context.ExchangeRates.AsNoTracking().ToListAsync();
        return entities.Select(e => new ExchangeRateEntity
        {
            CurrencyCode = e.CurrencyCode,
            Rate = e.Rate
        }).ToList();
    }

    public async Task<IReadOnlyList<ExchangeRateEntity>> GetExchangeRatesFromNbp()
    {
        var client = _httpClientFactory.CreateClient(HttpClientName);
        try
        {
            var response = await client.GetAsync(TableBUrl);

            var tables = await response.Content.ReadFromJsonAsync<NbpApiResponse[]>(JsonOptions);
            if (tables is null || tables.Length == 0)
            {
                return new List<ExchangeRateEntity>();
            }

            var table = tables.First();
            var rates = new List<ExchangeRateEntity>();
            foreach (var r in table.Rates)
            {
                rates.Add(new ExchangeRateEntity
                {
                    CurrencyCode = r.Code,
                    Rate = r.Mid
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

    public async Task SaveExchangeRatesAsync(IReadOnlyList<ExchangeRateEntity> exchangeRates)
    {
        await _context.ExchangeRates.ExecuteDeleteAsync();

        var entities = exchangeRates.Select(dto => new ExchangeRateEntity
        {
            CurrencyCode = dto.CurrencyCode,
            Rate = dto.Rate
        }).ToList();

        if (!entities.Any(e => e.CurrencyCode == "PLN"))
        {
            entities.Add(new ExchangeRateEntity { CurrencyCode = "PLN", Rate = 1 });
        }

        await _context.ExchangeRates.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DoesCurrencyExists(string currencyCode)
    {
        return await _context.ExchangeRates.AnyAsync(e => e.CurrencyCode == currencyCode);
    }

    public async Task<decimal?> GetExchangeRateFromDb(string currencyCode)
    {
        var entity = await _context.ExchangeRates
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.CurrencyCode == currencyCode);
        return entity?.Rate;
    }
}
