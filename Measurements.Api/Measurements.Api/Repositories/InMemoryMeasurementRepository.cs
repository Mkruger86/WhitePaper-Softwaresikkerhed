using System.Collections.Concurrent;
using Measurements.Api.Models;

namespace Measurements.Api.Repositories;

// Den konkrete implementation af IMeasurementRepositoryHer, hvor målinger persisteres midlertidigt in-memory som nævnt. 
// Den bruges efter validering og kan hente både enkelte målinger (Record) og samlet liste (Payload) 
// Alle data eksisterer midlertidligt for nu og forsvinder når applikationen genstartes, givet dette er en testimplementering.

public sealed class InMemoryMeasurementRepository : IMeasurementRepository
{
    private readonly ConcurrentDictionary<Guid, MeasurementRecord> _records = new();

    public MeasurementRecord Create(MeasurementPayload payload)
    {
        if (payload.Hits is null)
        {
            throw new ArgumentException(
                "Payload skal være valideret før lagring.",
                nameof(payload));
        }

        MeasurementRecord record = new MeasurementRecord
        {
            Id = Guid.NewGuid(),
            Hits = payload.Hits.Select(CloneHit).ToList(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _records[record.Id] = record;

        return record;
    }

    public MeasurementRecord? GetById(Guid id)
    {
        return _records.TryGetValue(id, out MeasurementRecord? record)
            ? record
            : null;
    }

    public IReadOnlyCollection<MeasurementRecord> List()
    {
        return _records.Values
            .OrderBy(record => record.CreatedAt)
            .ToList();
    }

    private static ARHit CloneHit(ARHit hit)
    {
        return new ARHit
        {
            X = hit.X,
            Y = hit.Y,
            Z = hit.Z,
            HitType = hit.HitType,
            Timestamp = hit.Timestamp
        };
    }
}