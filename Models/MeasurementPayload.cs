using System.Text.Json.Serialization;

namespace Measurements.Api.Models;

public class MeasurementPayload
{
    [JsonPropertyName("hits")]
    public List<ARHit>? Hits { get; set; }
}