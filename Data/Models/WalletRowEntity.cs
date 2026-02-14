namespace Wallet.Api.Data.Models;

public class WalletRowEntity
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public WalletEntity Wallet { get; set; } = null!;

    public string CurrencyCode { get; set; } = string.Empty;

    public ExchangeRateEntity Currency { get; set; } = null!;

    public decimal Amount { get; set; }
}
