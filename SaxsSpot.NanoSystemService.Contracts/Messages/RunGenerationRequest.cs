using System.Text.Json.Serialization;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Contracts.Messages;

/// <summary>
/// Request for nanosystem generation from Kafka
/// </summary>
public record RunGenerationRequest
{
    [JsonPropertyName("operationId")]
    public Guid OperationId { get; set; }
    
    [JsonPropertyName("seriesId")]
    public Guid SeriesId { get; set; }
    
    [JsonPropertyName("parameters")]
    public CommonParticleGenerationParameters Parameters { get; init; } = default!;
}
