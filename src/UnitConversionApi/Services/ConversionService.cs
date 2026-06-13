using UnitConversionApi.Data;
using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public class ConversionService : IConversionService
{
    public ConversionResult? Convert(ConversionRequest request)
    {
        var fromUnit = UnitRegistry.Find(request.FromUnit);
        var toUnit   = UnitRegistry.Find(request.ToUnit);

        // Unknown unit(s)
        if (fromUnit is null || toUnit is null)
            return null;

        // Cross-category conversion is meaningless
        if (!fromUnit.Category.Equals(toUnit.Category, StringComparison.OrdinalIgnoreCase))
            return null;

        // Two-step conversion: value → base unit → target unit
        var inBase  = fromUnit.ToBase(request.Value);
        var result  = toUnit.FromBase(inBase);

        return new ConversionResult
        {
            InputValue  = request.Value,
            FromUnit    = fromUnit.Name,
            OutputValue = result,
            ToUnit      = toUnit.Name,
            Category    = fromUnit.Category
        };
    }

    public IEnumerable<string> GetCategories() =>
        UnitRegistry.GetCategories();

    public IEnumerable<object> GetUnits(string? category = null) =>
        UnitRegistry.GetAll(category)
                    .Select(u => new
                    {
                        u.Name,
                        u.Category,
                        Aliases = u.Aliases
                    });
}