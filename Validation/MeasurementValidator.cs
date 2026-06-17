using System.Globalization;
using System.Text.Json;
using Measurements.Api.Models;

namespace Measurements.Api.Validation;

public sealed class MeasurementValidator
{
    private readonly HashSet<string> _allowedHitTypes;

    public MeasurementValidator(IEnumerable<string> allowedHitTypes)
    {
        _allowedHitTypes = allowedHitTypes.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public MeasurementValidationResult Validate(
    MeasurementPayload? payload,
    int payloadBytes,
    int repeatCount = 1)
    {
        if (payload is null)
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.ValueError,
                "Payload mangler.");
        }

        if (payloadBytes > MeasurementLimits.MAX_BYTES)
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.PayloadTooLarge,
                "Payload overstiger MAX_BYTES.");
        }

        if (payload.Hits is null)
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.ValueError,
                "hits mangler.");
        }

        if (payload.Hits.Count == 0)
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.ValueError,
                "hits.length skal være mindst 1.");
        }

        if (payload.Hits.Count > MeasurementLimits.MAX_HITS)
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.PayloadTooLarge,
                "hits.length overstiger MAX_HITS.");
        }

        if (repeatCount > MeasurementLimits.REPEAT_LIMIT)
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.ResourceLimitExceeded,
                "repeatCount overstiger REPEAT_LIMIT.");
        }

        foreach (ARHit hit in payload.Hits)
        {
            MeasurementValidationResult hitResult = ValidateHit(hit);

            if (!hitResult.IsValid)
            {
                return hitResult;
            }
        }

        return MeasurementValidationResult.Ok();
    }

    private MeasurementValidationResult ValidateHit(ARHit? hit)
    {
        if (hit is null)
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.ValueError,
                "ARHit mangler.");
        }

        if (!TryReadFiniteDouble(hit.X, out double x) ||
            !TryReadFiniteDouble(hit.Y, out double y) ||
            !TryReadFiniteDouble(hit.Z, out double z))
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.ValueError,
                "x, y og z skal være numeriske og finite værdier.");
        }

        bool coordinatesValid =
            x >= MeasurementLimits.MIN_COORD &&
            x <= MeasurementLimits.MAX_COORD &&
            y >= MeasurementLimits.MIN_COORD &&
            y <= MeasurementLimits.MAX_COORD &&
            z >= MeasurementLimits.MIN_COORD &&
            z <= MeasurementLimits.MAX_COORD;

        if (!coordinatesValid)
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.ValueError,
                "coordinatesValid = false.");
        }

        if (string.IsNullOrWhiteSpace(hit.HitType) ||
            !_allowedHitTypes.Contains(hit.HitType))
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.ValueError,
                "hitType er ikke tilladt.");
        }

        if (!TryReadTimestamp(hit.Timestamp))
        {
            return MeasurementValidationResult.Fail(
                MeasurementValidationError.ValueError,
                "timestampValid = false.");
        }

        return MeasurementValidationResult.Ok();
    }

    private static bool TryReadFiniteDouble(object? value, out double number)
    {
        number = default;

        switch (value)
        {
            case double doubleValue:
                number = doubleValue;
                return double.IsFinite(number);

            case float floatValue:
                number = floatValue;
                return double.IsFinite(number);

            case decimal decimalValue:
                number = (double)decimalValue;
                return double.IsFinite(number);

            case int intValue:
                number = intValue;
                return true;

            case long longValue:
                number = longValue;
                return true;

            case JsonElement jsonElement:
                return TryReadFiniteDoubleFromJsonElement(jsonElement, out number);

            default:
                return false;
        }
    }

    private static bool TryReadFiniteDoubleFromJsonElement(
        JsonElement jsonElement,
        out double number)
    {
        number = default;

        if (jsonElement.ValueKind != JsonValueKind.Number)
        {
            return false;
        }

        if (!jsonElement.TryGetDouble(out number))
        {
            return false;
        }

        return double.IsFinite(number);
    }

    private static bool TryReadTimestamp(object? value)
    {
        switch (value)
        {
            case DateTimeOffset:
                return true;

            case DateTime:
                return true;

            case string stringValue:
                return DateTimeOffset.TryParse(
                    stringValue,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out _);

            case JsonElement jsonElement:
                return TryReadTimestampFromJsonElement(jsonElement);

            default:
                return false;
        }
    }

    private static bool TryReadTimestampFromJsonElement(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        string? timestamp = jsonElement.GetString();

        return DateTimeOffset.TryParse(
            timestamp,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out _);
    }
}