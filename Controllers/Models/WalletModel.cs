namespace Wallet.Api.Controllers.Models;

public class WalletModel
{
    public Guid Id { get; set; }

    public string WalletName { get; set; } = string.Empty;

    public List<WalletRowModel> Rows { get; set; } = new();
}