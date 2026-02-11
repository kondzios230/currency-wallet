using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Wallet.Api.Data;
using Wallet.Api.Data.Interfaces;
using Wallet.Api.Services;
using Wallet.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    });

builder.Services.AddHttpClient("Nbp", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddScoped<IExchangeRatesService, ExchangeRatesService>();
builder.Services.AddScoped<IWalletService, WalletService>();

builder.Services.AddScoped<IExchangeRatesDataService, ExchangeRateDataService>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IWalletDataService, WalletDataService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.UseRouting();
app.MapControllers();

app.Run();
