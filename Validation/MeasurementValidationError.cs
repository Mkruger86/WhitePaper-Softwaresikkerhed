namespace Measurements.Api.Validation;

public enum MeasurementValidationError
{
    None,
    ValueError,
    PayloadTooLarge,
    ResourceLimitExceeded
}
