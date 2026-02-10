
namespace Wallet.Api.DTOs;

public class ExchangeRateDto
{
    public string CurrencyCode { get; set; } = string.Empty;

    public decimal ExchangeRate { get; set; }
}
