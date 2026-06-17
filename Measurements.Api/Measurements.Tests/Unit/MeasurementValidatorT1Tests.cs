using Measurements.Api.Models;
using Measurements.Api.Validation;

namespace Measurements.Tests.Unit;

public class MeasurementValidatorT1Tests
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

    private static MeasurementPayload CreateValidPayload()
    {
        return new MeasurementPayload
        {
            Hits = new List<ARHit>
            {
                new ARHit
                {
                    X = 0.0,
                    Y = 0.0,
                    Z = 0.0,
                    HitType = "plane",
                    Timestamp = DateTimeOffset.UtcNow.ToString("O")
                }
            }
        };
    }

    [Test]
    public void Validate_WhenHitsMissing_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = new MeasurementPayload
        {
            Hits = null
        };

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenHitsLengthIsZero_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = new MeasurementPayload
        {
            Hits = new List<ARHit>()
        };

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenCoordinateIsOutsideAllowedRange_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = MeasurementLimits.MAX_COORD + 1.0;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenHitTypeIsUnknown_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].HitType = "unknown";

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenTimestampIsInvalid_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Timestamp = "invalid-timestamp";

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenPayloadIsValid_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenCoordinateIsMissing_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = null;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenCoordinateIsString_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = "invalid-coordinate";

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenCoordinateIsBool_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = true;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenCoordinateIsNaN_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = double.NaN;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenCoordinateIsInfinity_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = double.PositiveInfinity;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenCoordinateEqualsMinCoord_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = MeasurementLimits.MIN_COORD;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenCoordinateIsBelowMinCoord_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = MeasurementLimits.MIN_COORD - 1.0;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenCoordinateEqualsMaxCoord_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = MeasurementLimits.MAX_COORD;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenHitsLengthIsTwo_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = new MeasurementPayload
        {
            Hits = new List<ARHit>
        {
            new ARHit
            {
                X = 0.0,
                Y = 0.0,
                Z = 0.0,
                HitType = "plane",
                Timestamp = DateTimeOffset.UtcNow.ToString("O")
            },
            new ARHit
            {
                X = 0.0,
                Y = 0.0,
                Z = 0.0,
                HitType = "point",
                Timestamp = DateTimeOffset.UtcNow.ToString("O")
            }
        }
        };

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenCoordinateEqualsZero_ReturnsOk()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].X = 0.0;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.None));
    }

    [Test]
    public void Validate_WhenYCoordinateIsMissing_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Y = null;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenZCoordinateIsMissing_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Z = null;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenYCoordinateIsBelowMinCoord_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Y = MeasurementLimits.MIN_COORD - 1.0;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenZCoordinateExceedsMaxCoord_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Z = MeasurementLimits.MAX_COORD + 1.0;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenYCoordinateIsNaN_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Y = double.NaN;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }

    [Test]
    public void Validate_WhenZCoordinateIsInfinity_ReturnsValueError()
    {
        MeasurementValidator validator = CreateValidator();

        MeasurementPayload payload = CreateValidPayload();
        payload.Hits![0].Z = double.PositiveInfinity;

        MeasurementValidationResult result = validator.Validate(
            payload,
            payloadBytes: 100);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Error, Is.EqualTo(MeasurementValidationError.ValueError));
    }
}