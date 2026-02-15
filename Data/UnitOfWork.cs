using Microsoft.EntityFrameworkCore.Storage;
using Wallet.Api.Data.Interfaces;

namespace Wallet.Api.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var efTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return new EfTransactionWrapper(efTransaction);
    }

    private sealed class EfTransactionWrapper : ITransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfTransactionWrapper(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default) =>
            _transaction.CommitAsync(cancellationToken);

        public ValueTask DisposeAsync() => _transaction.DisposeAsync();
    }
}
