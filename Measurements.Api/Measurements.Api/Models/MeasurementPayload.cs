using System.Text.Json.Serialization;

namespace Measurements.Api.Models;

// Repræsenterer hele payloadet fra klienten, når der oprettes en måling. 
// Payloadet samler de AR-hits, som brugeren eller enheden sender til APIet og indeholder da kun inputdata

public class MeasurementPayload
{
    [JsonPropertyName("hits")]
    public List<ARHit>? Hits { get; set; }
}