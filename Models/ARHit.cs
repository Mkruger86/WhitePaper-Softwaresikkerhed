using System.Text.Json.Serialization;

namespace Measurements.Api.Models;

public class ARHit
{
    [JsonPropertyName("x")]
    public object? X { get; set; }

    [JsonPropertyName("y")]
    public object? Y { get; set; }

    [JsonPropertyName("z")]
    public object? Z { get; set; }

    [JsonPropertyName("hitType")]
    public string? HitType { get; set; }

    [JsonPropertyName("timestamp")]
    public object? Timestamp { get; set; }
}
