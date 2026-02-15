namespace Wallet.Api.Data.Interfaces;

public interface IUnitOfWork
{
    ValueTask<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
