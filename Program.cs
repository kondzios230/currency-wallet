using System.Text.Json;
using Wallet.Api.Data;
using Wallet.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    });

builder.Services.AddScoped<IExchangeRatesService, ExchangeRatesService>();

builder.Services.AddSingleton<IExchangeRatesDataService, ExchangeRateDataService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
