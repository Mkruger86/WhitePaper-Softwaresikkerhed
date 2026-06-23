using Measurements.Api.Models;

namespace Measurements.Api.Services;

// Definerer de funktioner, som APIen bruger til at arbejde med målinger. 
// Meningen er her at skjule detaljerne om validering og lagring, således endpoints blot kan bede servicen om at oprette, hente eller liste målinger.

public interface IMeasurementService
{
    MeasurementCreateResult Create(
        MeasurementPayload? payload,
        int payloadBytes,
        int repeatCount = 1);

    MeasurementRecord? GetById(Guid id);

    IReadOnlyCollection<MeasurementRecord> List();
}