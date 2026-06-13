namespace UnitConversionApi.Models;

/// <summary>
/// Incoming payload for a unit conversion request.
/// </summary>
public record ConversionRequest
{
    /// <summary>The numeric value to convert.</summary>
    public double Value { get; init; }

    /// <summary>The unit to convert FROM (e.g. "meter", "celsius").</summary>
    public string FromUnit { get; init; } = string.Empty;

    /// <summary>The unit to convert TO (e.g. "foot", "fahrenheit").</summary>
    public string ToUnit { get; init; } = string.Empty;
}