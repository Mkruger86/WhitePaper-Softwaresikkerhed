using System.Text.Json.Serialization;

namespace Measurements.Api.Models;

// Repræsenterer et singulært AR hit, som Kollysion runtime sender fra Unity- eller
// mobilapplikationen.
//
// Klassen indeholder dataen for et registreret punkt: positionen i
// rummet, typen af hit og tidspunktet for registreringen. Dataene er endnu
// ikke godkendt, når de ligger i denne klasse.
//
// MeasurementValidator kontrollerer senere, at koordinaterne er numeriske
// og ligger inden for de tilladte grænser,
// at hitType er tilladt, og at timestampet kan læses korrekt.
// Derfra kan hittet indgå i en godkendt måling, som senere kan sendes videre til Firebase.

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
