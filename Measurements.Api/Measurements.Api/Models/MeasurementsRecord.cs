using System.Text.Json.Serialization;

namespace Measurements.Api.Models;

// Repræsenterer en måling, der er godkendt og gemt i systemet. 
// I modsætning til MeasurementPayload indeholder klassen her også systemets egne oplysninger,
// ergo id'et og tidspunktet målingen blev oprettet.

public class MeasurementRecord
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("hits")]
    public List<ARHit> Hits { get; init; } = new();

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; init; }
}