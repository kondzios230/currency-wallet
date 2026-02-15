using Wallet.Api.Services.Interfaces;

namespace Wallet.Api.Services;

public sealed class ExchangeRatesRefreshHostedService : BackgroundService
{
    private const int DefaultIntervalMinutes = 1;
    private const int MinIntervalMinutes = 1;
    private const int MaxIntervalMinutes = 60;

    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExchangeRatesRefreshHostedService> _logger;

    public ExchangeRatesRefreshHostedService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<ExchangeRatesRefreshHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMinutes = GetRefreshIntervalMinutes();

        _logger.LogInformation("Exchange rates refresh started. Interval: {Interval} minutes.", intervalMinutes);

        await RefreshRatesAsync();

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(intervalMinutes));
        while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
        {
            await RefreshRatesAsync();
        }
    }

    private async Task RefreshRatesAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IExchangeRatesService>();
            await service.RefreshExchangeRates().ConfigureAwait(false);
            _logger.LogDebug("Exchange rates refreshed successfully.");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exchange rates refresh failed. Previous rates remain in use.");
        }
    }

    private int GetRefreshIntervalMinutes()
    {
        var value = _configuration["ExchangeRates:RefreshIntervalMinutes"];
        if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out var minutes))
            return DefaultIntervalMinutes;

        return Math.Clamp(minutes, MinIntervalMinutes, MaxIntervalMinutes);
    }
}
