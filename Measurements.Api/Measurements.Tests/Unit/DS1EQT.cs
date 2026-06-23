using System.Collections.Generic;
using Measurements.Api.Models;
using Measurements.Api.Validation;
using NUnit.Framework;

namespace Measurements.Tests.Unit;

public class DS1EQT
{
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

    private static MeasurementPayload CreateValidPayload(int hitCount)
    {
        List<ARHit> hits = new();

        for (int i = 0; i < hitCount; i++)
        {
            hits.Add(
                new ARHit
                {
                    X = 1.0,
                    Y = 2.0,
                    Z = 3.0,
                    HitType = "plane",
                    Timestamp = ValidTimestamp
                });
        }

        return new MeasurementPayload
        {
            Hits = hits
        };
    }

    [Test]
    public void EC_D1_WhenPayloadBytesAndHitCountAreBelowLimits_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload(hitCount: 1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES / 2,
            repeatCount: 1);

        AssertOk(result);
    }

    [Test]
    public void EC_D2_WhenPayloadBytesEqualMaxBytes_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload(hitCount: 1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES,
            repeatCount: 1);

        AssertOk(result);
    }

    [Test]
    public void EC_D3_WhenPayloadBytesExceedMaxBytes_ReturnsPayloadTooLarge()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload(hitCount: 1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES + 100,
            repeatCount: 1);

        AssertPayloadTooLarge(result);
    }

    [Test]
    public void EC_D4_WhenHitCountEqualsMaxHits_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload(
            hitCount: MeasurementLimits.MAX_HITS);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES / 2,
            repeatCount: 1);

        AssertOk(result);
    }

    [Test]
    public void EC_D5_WhenHitCountExceedsMaxHits_ReturnsPayloadTooLarge()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload(
            hitCount: MeasurementLimits.MAX_HITS + 2);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES / 2,
            repeatCount: 1);

        AssertPayloadTooLarge(result);
    }

    [Test]
    public void EC_D6_WhenRepeatCountIsWithinLimit_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload(hitCount: 1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES / 2,
            repeatCount: 1);

        AssertOk(result);
    }

    [Test]
    public void EC_D7_WhenRepeatCountExceedsLimit_ReturnsResourceLimitExceeded()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload(hitCount: 1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES / 2,
            repeatCount: MeasurementLimits.REPEAT_LIMIT + 2);

        Assert.That(result.IsValid, Is.False);
        Assert.That(
            result.Error,
            Is.EqualTo(MeasurementValidationError.ResourceLimitExceeded));
    }

    private static void AssertOk(MeasurementValidationResult result)
    {
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    private static void AssertPayloadTooLarge(
        MeasurementValidationResult result)
    {
        Assert.That(result.IsValid, Is.False);
        Assert.That(
            result.Error,
            Is.EqualTo(MeasurementValidationError.PayloadTooLarge));
    }
}
