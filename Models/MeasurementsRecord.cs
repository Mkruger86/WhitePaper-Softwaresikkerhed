using System.Text.Json.Serialization;

namespace Measurements.Api.Models;

public class MeasurementRecord
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("hits")]
    public List<ARHit> Hits { get; init; } = new();

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; init; }
}