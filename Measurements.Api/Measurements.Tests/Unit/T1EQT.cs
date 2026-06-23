using System.Collections.Generic;
using Measurements.Api.Models;
using Measurements.Api.Validation;
using NUnit.Framework;

namespace Measurements.Tests.Unit;

public class T1EQT
{
    private const int ValidPayloadBytes = 500;
    private const string ValidTimestamp = "2026-01-01T12:00:00Z";

    private static MeasurementValidator CreateValidator()
    {
        return new MeasurementValidator(
            new[]
            {
                "plane",
                "point"
            });
    }

    private static MeasurementPayload CreateValidPayload()
    {
        return new MeasurementPayload
        {
            Hits = new List<ARHit>
            {
                new ARHit
                {
                    X = 1.0,
                    Y = 2.0,
                    Z = 3.0,
                    HitType = "plane",
                    Timestamp = ValidTimestamp
                }
            }
        };
    }

    [Test]
    public void EC_T1_WhenHitsAreMissing_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = new()
        {
            Hits = null
        };

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T2_WhenHitsLengthIsZero_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = new()
        {
            Hits = new List<ARHit>()
        };

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T4_WhenXIsNull_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = null;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T4_WhenYIsString_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Y = "not-a-number";

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T4_WhenZIsBool_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Z = true;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T5_WhenXIsNaN_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = double.NaN;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T5_WhenYIsPositiveInfinity_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Y = double.PositiveInfinity;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T5_WhenZIsNegativeInfinity_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Z = double.NegativeInfinity;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T6_WhenCoordinateIsOutsideAllowedRange_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = 100.0;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T7_WhenHitTypeIsUnknown_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].HitType = "unknown";

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T8_WhenTimestampIsInvalid_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Timestamp = "invalid-timestamp";

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        AssertValueError(result);
    }

    [Test]
    public void EC_T9_WhenPayloadContainsValidArHits_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    private static void AssertValueError(MeasurementValidationResult result)
    {
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }
}