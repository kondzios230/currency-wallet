using System.ComponentModel.DataAnnotations;

namespace Wallet.Api.Controllers.Models;

public class ExchangeRequestModel
{
    [Required(ErrorMessage = "Wallet ID is required.")]
    [NonDefaultGuid(ErrorMessage = "Wallet ID must not be empty.")]
    public Guid WalletId { get; set; }

    [Required(ErrorMessage = "Source currency code is required.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Source currency code must be exactly 3 characters.")]
    public string SourceCurrencyCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Target currency code is required.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Target currency code must be exactly 3 characters.")]
    public string TargetCurrencyCode { get; set; } = string.Empty;

    [Range(0.000001, 999_999_999.99, ErrorMessage = "Source amount must be positive.")]
    public decimal SourceAmount { get; set; }
}
