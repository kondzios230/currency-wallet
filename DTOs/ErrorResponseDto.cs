namespace Wallet.Api.DTOs;

/// <summary>Consistent error response shape (architecture: avoid leaking internals).</summary>
public class ErrorResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string? Code { get; set; }
}
