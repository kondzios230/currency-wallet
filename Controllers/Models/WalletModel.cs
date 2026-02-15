using System.ComponentModel.DataAnnotations;

namespace Wallet.Api.Controllers.Models;

public class WalletModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Wallet name is required.")]
    [MinLength(1, ErrorMessage = "Wallet name must not be empty.")]
    [MaxLength(200, ErrorMessage = "Wallet name must not exceed 200 characters.")]
    public string WalletName { get; set; } = string.Empty;

    public List<WalletRowModel> Rows { get; set; } = new();
}