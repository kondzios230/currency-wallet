using Microsoft.EntityFrameworkCore;
using Wallet.Api.Data.Models;

namespace Wallet.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ExchangeRateEntity> ExchangeRates => Set<ExchangeRateEntity>();
}
