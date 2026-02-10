using System.Text.Json.Serialization;

namespace Wallet.Api.Data;

public class NbpApiResponseLine
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("mid")]
    public decimal Mid { get; set; }
}
