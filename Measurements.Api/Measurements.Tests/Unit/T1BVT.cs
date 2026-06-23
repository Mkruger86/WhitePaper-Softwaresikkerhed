using System.Collections.Generic;
using Measurements.Api.Models;
using Measurements.Api.Validation;
using NUnit.Framework;

namespace Measurements.Tests.Unit;

public class T1BVT
{
    private const int ValidPayloadBytes =
        MeasurementLimits.MAX_BYTES / 2;

    private const int ValidRepeatCount =
        MeasurementLimits.REPEAT_LIMIT / 2;

    private const double ValidCoordinate =
        (MeasurementLimits.MIN_COORD + MeasurementLimits.MAX_COORD) / 2.0;

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

    private static MeasurementPayload CreatePayload(int hitCount)
    {
        List<ARHit> hits = new();

        for (int i = 0; i < hitCount; i++)
        {
            hits.Add(
                new ARHit
                {
                    X = ValidCoordinate,
                    Y = ValidCoordinate,
                    Z = ValidCoordinate,
                    HitType = "plane",
                    Timestamp = ValidTimestamp
                });
        }

        return new MeasurementPayload
        {
            Hits = hits
        };
    }

    [TestCase(
        0,
        false,
        MeasurementValidationError.ValueError,
        TestName = "BV_TL1_WhenHitsLengthIsZero_ReturnsValueError")]

    [TestCase(
        1,
        true,
        MeasurementValidationError.None,
        TestName = "BV_TL2_WhenHitsLengthIsOne_ReturnsOk")]

    [TestCase(
        2,
        true,
        MeasurementValidationError.None,
        TestName = "BV_TL3_WhenHitsLengthIsTwo_ReturnsOk")]
    public void HitsLengthBoundary_ReturnsExpectedResult(
        int hitCount,
        bool expectedValid,
        MeasurementValidationError expectedError)
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayload(hitCount);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes,
            repeatCount: ValidRepeatCount);

        Assert.That(result.IsValid, Is.EqualTo(expectedValid));
        Assert.That(result.Error, Is.EqualTo(expectedError));
    }

    [TestCase(
        MeasurementLimits.MIN_COORD - 1.0,
        false,
        MeasurementValidationError.ValueError,
        TestName = "BV_TC1_WhenXIsOneBelowMinCoord_ReturnsValueError")]

    [TestCase(
        MeasurementLimits.MIN_COORD,
        true,
        MeasurementValidationError.None,
        TestName = "BV_TC2_WhenXEqualsMinCoord_ReturnsOk")]

    [TestCase(
        0.0,
        true,
        MeasurementValidationError.None,
        TestName = "BV_TC3_WhenXEqualsZero_ReturnsOk")]

    [TestCase(
        MeasurementLimits.MAX_COORD,
        true,
        MeasurementValidationError.None,
        TestName = "BV_TC4_WhenXEqualsMaxCoord_ReturnsOk")]

    [TestCase(
        MeasurementLimits.MAX_COORD + 1.0,
        false,
        MeasurementValidationError.ValueError,
        TestName = "BV_TC5_WhenXIsOneAboveMaxCoord_ReturnsValueError")]
    
    public void XCoordinateBoundary_ReturnsExpectedResult(
        double x,
        bool expectedValid,
        MeasurementValidationError expectedError)
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayload(hitCount: 1);
        payload.Hits![0].X = x;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes,
            repeatCount: ValidRepeatCount);

        Assert.That(result.IsValid, Is.EqualTo(expectedValid));
        Assert.That(result.Error, Is.EqualTo(expectedError));
    }
}

