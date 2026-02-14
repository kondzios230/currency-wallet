namespace Wallet.Api.Data.Models;

public class WalletEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<WalletRowEntity> Rows { get; set; } = new List<WalletRowEntity>();
}
