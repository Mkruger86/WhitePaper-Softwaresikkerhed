using Measurements.Api.Models;

namespace Measurements.Api.Repositories;

// Definerer kontrakten for persistering af målingerne. 
// Service-laget bruger dette interface i stedet for at kende konkrete løsning/platform 
// På denne måde kan vi senere at udskifte denne nuværen in-memory implementering med aktuelle database (firebase pt), 
// uden at ændre resten af applikationens flow.

public interface IMeasurementRepository
{
    MeasurementRecord Create(MeasurementPayload payload);

    MeasurementRecord? GetById(Guid id);

    IReadOnlyCollection<MeasurementRecord> List();
}