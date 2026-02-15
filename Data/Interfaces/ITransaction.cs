namespace Wallet.Api.Data.Interfaces;

public interface ITransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
