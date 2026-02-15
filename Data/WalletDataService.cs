using Microsoft.EntityFrameworkCore;
using Wallet.Api.Data.Interfaces;
using Wallet.Api.Data.Models;

namespace Wallet.Api.Data;

public sealed class WalletDataService : IWalletDataService
{
    private readonly AppDbContext _context;

    public WalletDataService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WalletEntity?> CreateWallet(string name)
    {
        if (await _context.Wallets.AnyAsync(w => w.Name == name))
            return null;

        var entity = new WalletEntity
        {
            Id = Guid.NewGuid(),
            Name = name
        };
        _context.Wallets.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> RemoveWallet(Guid walletId)
    {
        var deleted = await _context.Wallets.Where(w => w.Id == walletId).ExecuteDeleteAsync();
        return deleted > 0;
    }

    public async Task<WalletRowEntity?> AddWalletRow(Guid walletId, string currencyCode, decimal amount)
    {
        var walletExists = await _context.Wallets.AnyAsync(w => w.Id == walletId);
        if (!walletExists)
            return null;

        var currencyExists = await _context.ExchangeRates.AnyAsync(e => e.CurrencyCode == currencyCode);
        if (!currencyExists)
            return null;

        var entity = new WalletRowEntity
        {
            Id = Guid.NewGuid(),
            WalletId = walletId,
            CurrencyCode = currencyCode,
            Amount = amount
        };
        _context.WalletRows.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> RemoveWalletRow(Guid walletRowId)
    {
        var deleted = await _context.WalletRows.Where(r => r.Id == walletRowId).ExecuteDeleteAsync();
        return deleted > 0;
    }

    public async Task<bool> WalletExists(Guid id)
    {
        return await _context.Wallets.AnyAsync(w => w.Id == id);
    }

    public async Task<WalletEntity?> GetWallet(Guid id)
    {
        return await _context.Wallets
            .Include(w => w.Rows)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<WalletRowEntity?> GetWalletRow(Guid walletId, string currencyCode)
    {
        return await _context.WalletRows
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.WalletId == walletId && r.CurrencyCode == currencyCode);
    }

    public async Task<int> IncrementWalletRowAmount(Guid walletId, string currencyCode, decimal amount)
    {
        return await _context.WalletRows
            .Where(r => r.WalletId == walletId && r.CurrencyCode == currencyCode)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.Amount, r => r.Amount + amount));
    }

    public async Task<int> DecrementWalletRowAmount(Guid walletId, string currencyCode, decimal amount)
    {
        return await _context.WalletRows
            .Where(r => r.WalletId == walletId && r.CurrencyCode == currencyCode && r.Amount >= amount)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.Amount, r => r.Amount - amount));
    }

    public async Task<IReadOnlyList<WalletEntity>> GetAllWallets()
    {
        return await _context.Wallets
            .Include(w => w.Rows)
            .ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
