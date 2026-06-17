using Measurements.Api.Models;

namespace Measurements.Api.Repositories;

public interface IMeasurementRepository
{
    MeasurementRecord Create(MeasurementPayload payload);

    MeasurementRecord? GetById(Guid id);

    IReadOnlyCollection<MeasurementRecord> List();
}