using Measurements.Api.Models;
using Measurements.Api.Validation;

namespace Measurements.Tests.Unit;

public class MeasurementValidatorDS1Tests
{
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
            X = 0.0,
            Y = 0.0,
            Z = 0.0,
            HitType = "plane",
            Timestamp = DateTimeOffset.UtcNow.ToString("O")
        };
    }

    private static MeasurementPayload CreatePayloadWithHitCount(int hitCount)
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

    [Test]
    public void Validate_WhenPayloadBytesAreBelowMaxBytes_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayloadWithHitCount(1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES - 1);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenPayloadBytesEqualMaxBytes_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayloadWithHitCount(1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenPayloadBytesExceedMaxBytes_ReturnsPayloadTooLarge()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayloadWithHitCount(1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: MeasurementLimits.MAX_BYTES + 1);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.PayloadTooLarge));
    }

    [Test]
    public void Validate_WhenHitsLengthEqualsMaxHits_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayloadWithHitCount(
            MeasurementLimits.MAX_HITS);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenHitsLengthExceedsMaxHits_ReturnsPayloadTooLarge()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayloadWithHitCount(
            MeasurementLimits.MAX_HITS + 1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.PayloadTooLarge));
    }

    [Test]
    public void Validate_WhenRepeatCountEqualsRepeatLimit_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayloadWithHitCount(1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100,
            repeatCount: MeasurementLimits.REPEAT_LIMIT);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenRepeatCountExceedsRepeatLimit_ReturnsResourceLimitExceeded()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayloadWithHitCount(1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100,
            repeatCount: MeasurementLimits.REPEAT_LIMIT + 1);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ResourceLimitExceeded));
    }

    [Test]
    public void Validate_WhenHitsLengthIsOneBelowMaxHits_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayloadWithHitCount(
            MeasurementLimits.MAX_HITS - 1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenRepeatCountIsOneBelowRepeatLimit_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreatePayloadWithHitCount(1);

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100,
            repeatCount: MeasurementLimits.REPEAT_LIMIT - 1);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }
}