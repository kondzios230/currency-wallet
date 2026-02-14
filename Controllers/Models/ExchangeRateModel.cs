namespace Wallet.Api.Controllers.Models;

public class ExchangeRateModel
{
    public string CurrencyCode { get; set; } = string.Empty;

    public decimal ExchangeRate { get; set; }
}
