namespace Wallet.Api.Controllers.Models;

public class WalletRowModel
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}
