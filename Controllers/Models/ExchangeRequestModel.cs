namespace Wallet.Api.Controllers.Models;

public class ExchangeRequestModel
{
    public Guid WalletId { get; set; }

    public string SourceCurrencyCode { get; set; } = string.Empty;

    public string TargetCurrencyCode { get; set; } = string.Empty;

    public decimal SourceAmount { get; set; }
}
