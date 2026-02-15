using Microsoft.EntityFrameworkCore;
using Wallet.Api.Data.Interfaces;
using Wallet.Api.Data.Models;

namespace Wallet.Api.Data;

public sealed class WalletDataService : IWalletDataService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public WalletDataService(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<WalletEntity?> CreateWallet(string name, CancellationToken cancellationToken = default)
    {
        if (await _context.Wallets.AnyAsync(w => w.Name == name, cancellationToken))
            return null;

        var entity = new WalletEntity
        {
            Id = Guid.NewGuid(),
            Name = name
        };
        _context.Wallets.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> RemoveWallet(Guid walletId, CancellationToken cancellationToken = default)
    {
        var deleted = await _context.Wallets.Where(w => w.Id == walletId).ExecuteDeleteAsync(cancellationToken);
        return deleted > 0;
    }

    public async Task<WalletRowEntity?> AddWalletRow(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default)
    {
        var walletExists = await _context.Wallets.AnyAsync(w => w.Id == walletId, cancellationToken);
        if (!walletExists)
            return null;

        var currencyExists = await _context.ExchangeRates.AnyAsync(e => e.CurrencyCode == currencyCode, cancellationToken);
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
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> RemoveWalletRow(Guid walletRowId, CancellationToken cancellationToken = default)
    {
        var deleted = await _context.WalletRows.Where(r => r.Id == walletRowId).ExecuteDeleteAsync(cancellationToken);
        return deleted > 0;
    }

    public async Task<bool> WalletExists(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AnyAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<WalletEntity?> GetWallet(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .Include(w => w.Rows)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<WalletRowEntity?> GetWalletRow(Guid walletId, string currencyCode, CancellationToken cancellationToken = default)
    {
        return await _context.WalletRows
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.WalletId == walletId && r.CurrencyCode == currencyCode, cancellationToken);
    }

    public async Task<int> IncrementWalletRowAmount(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default)
    {
        return await _context.WalletRows
            .Where(r => r.WalletId == walletId && r.CurrencyCode == currencyCode)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.Amount, r => r.Amount + amount), cancellationToken);
    }

    public async Task<int> DecrementWalletRowAmount(Guid walletId, string currencyCode, decimal amount, CancellationToken cancellationToken = default)
    {
        return await _context.WalletRows
            .Where(r => r.WalletId == walletId && r.CurrencyCode == currencyCode && r.Amount >= amount)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.Amount, r => r.Amount - amount), cancellationToken);
    }

    public async Task<IReadOnlyList<WalletEntity>> GetAllWallets(CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .Include(w => w.Rows)
            .ToListAsync(cancellationToken);
    }

    public async Task Exchange(Guid walletId, string sourceCurrencyCode, string targetCurrencyCode, decimal sourceAmount, decimal targetAmount, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var rowsUpdated = await DecrementWalletRowAmount(walletId, sourceCurrencyCode, sourceAmount, cancellationToken);
        if (rowsUpdated == 0)
            throw new InvalidOperationException("Source currency does not exist in this wallet or insufficient balance.");

        var sourceRow = await GetWalletRow(walletId, sourceCurrencyCode, cancellationToken);
        if (sourceRow != null && sourceRow.Amount == 0)
            await RemoveWalletRow(sourceRow.Id, cancellationToken);

        var targetRowsUpdated = await IncrementWalletRowAmount(walletId, targetCurrencyCode, targetAmount, cancellationToken);
        if (targetRowsUpdated == 0)
            await AddWalletRow(walletId, targetCurrencyCode, targetAmount, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
