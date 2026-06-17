using Measurements.Api.Models;
using Measurements.Api.Repositories;
using Measurements.Api.Validation;

namespace Measurements.Api.Services;

public sealed class MeasurementService : IMeasurementService
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