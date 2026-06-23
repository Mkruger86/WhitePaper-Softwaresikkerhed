using System.Collections.Generic;
using Measurements.Api.Models;
using Measurements.Api.Validation;
using NUnit.Framework;

namespace Measurements.Tests.Unit;

public class DS1BVT
{
    private const int ValidPayloadBytes =
        MeasurementLimits.MAX_BYTES / 2;

    private const int ValidRepeatCount =
        MeasurementLimits.REPEAT_LIMIT / 2;

    private const int ValidHitCount =
        MeasurementLimits.MAX_HITS / 2;

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

    private static ARHit CreateValidHit()
    {
        return new ARHit
        {
            X = ValidCoordinate,
            Y = ValidCoordinate,
            Z = ValidCoordinate,
            HitType = "plane",
            Timestamp = ValidTimestamp
        };
    }

    private static MeasurementPayload CreatePayload(int hitCount)
    {
        List<ARHit> hits = new();

        for (int i = 0; i < hitCount; i++)
        {
            hits.Add(CreateValidHit());
        }

        return new MeasurementPayload
        {
            Hits = hits
        };
    }

    [TestCase(
        MeasurementLimits.MAX_HITS - 1,
        true,
        MeasurementValidationError.None,
        TestName = "BV_DH1_WhenHitsLengthIsOneBelowMaxHits_ReturnsOk")]

    [TestCase(
        MeasurementLimits.MAX_HITS,
        true,
        MeasurementValidationError.None,
        TestName = "BV_DH2_WhenHitsLengthEqualsMaxHits_ReturnsOk")]

    [TestCase(
        MeasurementLimits.MAX_HITS + 1,
        false,
        MeasurementValidationError.PayloadTooLarge,
        TestName = "BV_DH3_WhenHitsLengthIsOneAboveMaxHits_ReturnsPayloadTooLarge")]
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
        MeasurementLimits.MAX_BYTES - 1,
        true,
        MeasurementValidationError.None,
        TestName = "BV_DP1_WhenPayloadBytesAreOneBelowMaxBytes_ReturnsOk")]

    [TestCase(
        MeasurementLimits.MAX_BYTES,
        true,
        MeasurementValidationError.None,
        TestName = "BV_DP2_WhenPayloadBytesEqualMaxBytes_ReturnsOk")]

    [TestCase(
        MeasurementLimits.MAX_BYTES + 1,
        false,
        MeasurementValidationError.PayloadTooLarge,
        TestName = "BV_DP3_WhenPayloadBytesAreOneAboveMaxBytes_ReturnsPayloadTooLarge")]
    public void PayloadBytesBoundary_ReturnsExpectedResult(
        int payloadBytes,
        bool expectedValid,
        MeasurementValidationError expectedError)
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayload(ValidHitCount);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: payloadBytes,
            repeatCount: ValidRepeatCount);

        Assert.That(result.IsValid, Is.EqualTo(expectedValid));
        Assert.That(result.Error, Is.EqualTo(expectedError));
    }

    [TestCase(
        MeasurementLimits.REPEAT_LIMIT - 1,
        true,
        MeasurementValidationError.None,
        TestName = "BV_DR1_WhenRepeatCountIsOneBelowRepeatLimit_ReturnsOk")]

    [TestCase(
        MeasurementLimits.REPEAT_LIMIT,
        true,
        MeasurementValidationError.None,
        TestName = "BV_DR2_WhenRepeatCountEqualsRepeatLimit_ReturnsOk")]

    [TestCase(
        MeasurementLimits.REPEAT_LIMIT + 1,
        false,
        MeasurementValidationError.ResourceLimitExceeded,
        TestName = "BV_DR3_WhenRepeatCountIsOneAboveRepeatLimit_ReturnsResourceLimitExceeded")]
    public void RepeatCountBoundary_ReturnsExpectedResult(
        int repeatCount,
        bool expectedValid,
        MeasurementValidationError expectedError)
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayload(ValidHitCount);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: ValidPayloadBytes,
            repeatCount: repeatCount);

        Assert.That(result.IsValid, Is.EqualTo(expectedValid));
        Assert.That(result.Error, Is.EqualTo(expectedError));
    }
}