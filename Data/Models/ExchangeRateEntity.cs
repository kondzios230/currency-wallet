namespace Wallet.Api.Data.Models;

public class ExchangeRateEntity
{
    public int Id { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public decimal Rate { get; set; }
}
