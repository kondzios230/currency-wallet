namespace Wallet.Api.Controllers.Models;

public class ModifyRowModel
{
    public Guid WalletId { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}
