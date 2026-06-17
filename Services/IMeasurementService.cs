using Measurements.Api.Models;

namespace Measurements.Api.Services;

public interface IMeasurementService
{
    MeasurementCreateResult Create(
        MeasurementPayload? payload,
        int payloadBytes,
        int repeatCount = 1);

    MeasurementRecord? GetById(Guid id);

    IReadOnlyCollection<MeasurementRecord> List();
}