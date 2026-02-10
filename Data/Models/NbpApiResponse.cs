using System.Text.Json.Serialization;

namespace Wallet.Api.Data;

public class NbpApiResponse
{
    [JsonPropertyName("table")]
    public string Table { get; set; } = string.Empty;

    [JsonPropertyName("no")]
    public string No { get; set; } = string.Empty;

    [JsonPropertyName("effectiveDate")]
    public string EffectiveDate { get; set; } = string.Empty;

    [JsonPropertyName("rates")]
    public List<NbpApiResponseLine> Rates { get; set; } = new();
}