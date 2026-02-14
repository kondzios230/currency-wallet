namespace Wallet.Api.Services.Models;

public class WalletDto
{
    public Guid Id { get; set; }

    public string WalletName { get; set; } = string.Empty;

    public List<WalletRowDto> Rows { get; set; } = new();
}