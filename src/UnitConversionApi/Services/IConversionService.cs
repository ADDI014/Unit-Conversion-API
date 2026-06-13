using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public interface IConversionService
{
    /// <summary>
    /// Converts a value from one unit to another.
    /// Returns null if either unit is unknown or they belong to different categories.
    /// </summary>
    ConversionResult? Convert(ConversionRequest request);

    /// <summary>Returns all supported categories.</summary>
    IEnumerable<string> GetCategories();

    /// <summary>Returns all supported units, optionally filtered by category.</summary>
    IEnumerable<object> GetUnits(string? category = null);
}