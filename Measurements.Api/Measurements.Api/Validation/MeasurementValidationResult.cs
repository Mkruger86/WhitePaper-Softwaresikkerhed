namespace Measurements.Api.Validation;

public sealed class MeasurementValidationResult
{
    public bool IsValid { get; }
    public MeasurementValidationError Error { get; }
    public string? Message { get; }

    private MeasurementValidationResult(
        bool isValid,
        MeasurementValidationError error,
        string? message)
    {
        IsValid = isValid;
        Error = error;
        Message = message;
    }

    public static MeasurementValidationResult Ok()
    {
        return new MeasurementValidationResult(
            true,
            MeasurementValidationError.None,
            null);
    }

    public static MeasurementValidationResult Fail(
        MeasurementValidationError error,
        string message)
    {
        return new MeasurementValidationResult(
            false,
            error,
            message);
    }
}