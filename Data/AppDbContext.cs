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
    public DbSet<WalletEntity> Wallets => Set<WalletEntity>();
    public DbSet<WalletRowEntity> WalletRows => Set<WalletRowEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExchangeRateEntity>(e =>
        {
            e.HasIndex(x => x.CurrencyCode).IsUnique();
        });

        modelBuilder.Entity<WalletEntity>(w =>
        {
            w.HasKey(x => x.Id);
            w.HasIndex(x => x.Name).IsUnique();
            w.HasMany(x => x.Rows)
                .WithOne(x => x.Wallet)
                .HasForeignKey(x => x.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WalletRowEntity>(r =>
        {
            r.HasKey(x => x.Id);
            r.Property(x => x.Amount).HasPrecision(18, 2);
            r.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyCode)
                .HasPrincipalKey(e => e.CurrencyCode)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
