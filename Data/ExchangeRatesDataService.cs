using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Wallet.Api.Data.Interfaces;
using Wallet.Api.Data.Models;
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
    private readonly AppDbContext _context;

    public ExchangeRateDataService(IHttpClientFactory httpClientFactory, AppDbContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRatesFromDB()
    {
        var entities = await _context.ExchangeRates.AsNoTracking().ToListAsync();
        return entities.Select(e => new ExchangeRateDto
        {
            CurrencyCode = e.CurrencyCode,
            ExchangeRate = e.Rate
        }).ToList();
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRatesFromNbp()
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

    public async Task SaveExchangeRatesAsync(IReadOnlyList<ExchangeRateDto> exchangeRates)
    {
        await _context.ExchangeRates.ExecuteDeleteAsync();

        var entities = exchangeRates.Select(dto => new ExchangeRateEntity
        {
            CurrencyCode = dto.CurrencyCode,
            Rate = dto.ExchangeRate
        });

        await _context.ExchangeRates.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
