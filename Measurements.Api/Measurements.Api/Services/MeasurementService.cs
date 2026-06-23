using Measurements.Api.Models;
using Measurements.Api.Repositories;
using Measurements.Api.Validation;

namespace Measurements.Api.Services;

// Centrummet for oprettelse af målinger. 
// Klassen modtager data fra API-laget, sender dem til validering og gemmer dem 
// kun gennem repository-laget, hvis de opfylder reglerne. Den binder dermed 
// validatoren og datalagringen sammen uden at blande deres ansvar sammen.

public class MeasurementService : IMeasurementService
{
    private readonly MeasurementValidator _validator;
    private readonly IMeasurementRepository _repository;

    public MeasurementService(
        MeasurementValidator validator,
        IMeasurementRepository repository)
    {
        _validator = validator;
        _repository = repository;
    }

    public MeasurementCreateResult Create(
        MeasurementPayload? payload,
        int payloadBytes,
        int repeatCount = 1)
    {
        MeasurementValidationResult validationResult = _validator.Validate(
            payload,
            payloadBytes,
            repeatCount);

        if (!validationResult.IsValid)
        {
            return MeasurementCreateResult.Fail(
                validationResult.Error,
                validationResult.Message);
        }

        MeasurementRecord record = _repository.Create(payload!);

        return MeasurementCreateResult.Success(record);
    }

    public MeasurementRecord? GetById(Guid id)
    {
        return _repository.GetById(id);
    }

    public IReadOnlyCollection<MeasurementRecord> List()
    {
        return _repository.List();
    }
}