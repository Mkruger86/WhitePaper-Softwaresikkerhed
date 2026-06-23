using Measurements.Api.Models;
using Measurements.Api.Validation;

namespace Measurements.Api.Services;

// Indeholder resultatet af et forsøg på at oprette en måling.
// Klassen bruges til at sende valideringsfejl tilbage som en returværdi,
// så kaldende kode og tests kan kontrollere forskellige custom fejltyper, uden at skulle forvente og fange exceptions

public class MeasurementCreateResult
{
    public bool IsSuccess { get; }
    public MeasurementRecord? Record { get; }
    public MeasurementValidationError Error { get; }
    public string? Message { get; }

    private MeasurementCreateResult(
        bool isSuccess,
        MeasurementRecord? record,
        MeasurementValidationError error,
        string? message)
    {
        IsSuccess = isSuccess;
        Record = record;
        Error = error;
        Message = message;
    }

    public static MeasurementCreateResult Success(MeasurementRecord record)
    {
        return new MeasurementCreateResult(
            true,
            record,
            MeasurementValidationError.None,
            null);
    }

    public static MeasurementCreateResult Fail(
        MeasurementValidationError error,
        string? message)
    {
        return new MeasurementCreateResult(
            false,
            null,
            error,
            message);
    }
}