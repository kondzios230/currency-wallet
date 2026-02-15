using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Wallet.Api.Data;
using Wallet.Api.Data.Interfaces;
using Wallet.Api.Services;
using Wallet.Api.Services.Interfaces;

namespace Wallet.Api;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
            });

        builder.Services.AddApiClient();
        builder.Services.AddDataServices(builder.Configuration);
        builder.Services.AddServices();
        builder.Services.AddHostedService<ExchangeRatesRefreshHostedService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllers();

        app.Run();
    }

    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IWalletDataService, WalletDataService>();
        services.AddScoped<IExchangeRatesDataService, ExchangeRatesDataService>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ExchangeRatesAccessLock>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IExchangeRatesService, ExchangeRatesService>();

        return services;
    }

    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services.AddHttpClient("Nbp", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        return services;
    }
}
