namespace Measurements.Api.Validation;

// Samler de faste regler for, hvor store og omfattende målinger må være. 
// Bruges direkte af MeasurementValidator

public static class MeasurementLimits
{
    public const double MIN_COORD = -10.0;
    public const double MAX_COORD = 10.0;

    public const int MAX_HITS = 3;
    public const int MAX_BYTES = 1024;
    public const int REPEAT_LIMIT = 3;
}