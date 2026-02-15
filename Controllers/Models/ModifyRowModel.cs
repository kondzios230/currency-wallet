using System.ComponentModel.DataAnnotations;

namespace Wallet.Api.Controllers.Models;

public class ModifyRowModel
{
    [Required(ErrorMessage = "Wallet ID is required.")]
    public Guid WalletId { get; set; }

    [Required(ErrorMessage = "Currency code is required.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters.")]
    public string CurrencyCode { get; set; } = string.Empty;

    [Range(0.000001, 999999999.99, ErrorMessage = "Amount must be positive.")]
    public decimal Amount { get; set; }
}
