namespace UnitConversionApi.Models;

/// <summary>
/// Response returned after a successful conversion.
/// </summary>
public record ConversionResult
{
    public double InputValue { get; init; }
    public string FromUnit { get; init; } = string.Empty;
    public double OutputValue { get; init; }
    public string ToUnit { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
}