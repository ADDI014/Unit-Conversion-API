namespace UnitConversionApi.Data;

/// <summary>
/// Describes a single unit: its canonical name, aliases, category,
/// and how it relates to the category's base unit.
///
/// Conversion formula:  toBase(value) converts the value INTO the base unit.
///                      fromBase(value) converts OUT of the base unit.
///
/// For linear units (most): fromBase = 1 / toBase factor.
/// For non-linear units (e.g. temperature): arbitrary lambdas.
/// </summary>
public record UnitDefinition(
    string Name,
    string Category,
    string[] Aliases,
    Func<double, double> ToBase,
    Func<double, double> FromBase
);

public static class UnitRegistry
{
    private static readonly List<UnitDefinition> _units = new()
    {
        // ── LENGTH (base = meter) ────────────────────────────────────────────
        new("meter",      "length", new[] { "m", "meters", "metre", "metres" },
            v => v, v => v),
        new("kilometer",  "length", new[] { "km", "kilometers", "kilometres" },
            v => v * 1000, v => v / 1000),
        new("centimeter", "length", new[] { "cm", "centimeters", "centimetres" },
            v => v / 100, v => v * 100),
        new("millimeter", "length", new[] { "mm", "millimeters", "millimetres" },
            v => v / 1000, v => v * 1000),
        new("mile",       "length", new[] { "mi", "miles" },
            v => v * 1609.344, v => v / 1609.344),
        new("yard",       "length", new[] { "yd", "yards" },
            v => v * 0.9144, v => v / 0.9144),
        new("foot",       "length", new[] { "ft", "feet" },
            v => v * 0.3048, v => v / 0.3048),
        new("inch",       "length", new[] { "in", "inches" },
            v => v * 0.0254, v => v / 0.0254),

        // ── TEMPERATURE (base = Celsius) ─────────────────────────────────────
        new("celsius",    "temperature", new[] { "c", "°c", "degc" },
            v => v, v => v),
        new("fahrenheit", "temperature", new[] { "f", "°f", "degf" },
            v => (v - 32) * 5 / 9, v => v * 9 / 5 + 32),
        new("kelvin",     "temperature", new[] { "k" },
            v => v - 273.15, v => v + 273.15),

        // ── WEIGHT / MASS (base = kilogram) ──────────────────────────────────
        new("kilogram",   "weight", new[] { "kg", "kilograms", "kilogramme", "kilogrammes" },
            v => v, v => v),
        new("gram",       "weight", new[] { "g", "grams", "gramme", "grammes" },
            v => v / 1000, v => v * 1000),
        new("milligram",  "weight", new[] { "mg", "milligrams" },
            v => v / 1_000_000, v => v * 1_000_000),
        new("pound",      "weight", new[] { "lb", "lbs", "pounds" },
            v => v * 0.453592, v => v / 0.453592),
        new("ounce",      "weight", new[] { "oz", "ounces" },
            v => v * 0.0283495, v => v / 0.0283495),
        new("tonne",      "weight", new[] { "t", "metric ton", "metric tons", "tonnes" },
            v => v * 1000, v => v / 1000),

        // ── VOLUME (base = liter) ─────────────────────────────────────────────
        new("liter",       "volume", new[] { "l", "liters", "litre", "litres" },
            v => v, v => v),
        new("milliliter",  "volume", new[] { "ml", "milliliters", "millilitre", "millilitres" },
            v => v / 1000, v => v * 1000),
        new("gallon",      "volume", new[] { "gal", "gallons", "us gallon", "us gallons" },
            v => v * 3.78541, v => v / 3.78541),
        new("quart",       "volume", new[] { "qt", "quarts" },
            v => v * 0.946353, v => v / 0.946353),
        new("pint",        "volume", new[] { "pt", "pints" },
            v => v * 0.473176, v => v / 0.473176),
        new("cup",         "volume", new[] { "cups" },
            v => v * 0.24, v => v / 0.24),
        new("fluid ounce", "volume", new[] { "fl oz", "fluid ounces", "floz" },
            v => v * 0.0295735, v => v / 0.0295735),
        new("cubic meter", "volume", new[] { "m3", "m³", "cubic meters", "cubic metres" },
            v => v * 1000, v => v / 1000),

        // ── SPEED (base = meters per second) ─────────────────────────────────
        new("meters per second", "speed", new[] { "m/s", "mps", "meters/second" },
            v => v, v => v),
        new("kilometers per hour", "speed", new[] { "km/h", "kmh", "kph", "kmph" },
            v => v / 3.6, v => v * 3.6),
        new("miles per hour",     "speed", new[] { "mph", "miles/hour" },
            v => v * 0.44704, v => v / 0.44704),
        new("knot",               "speed", new[] { "kt", "knots", "kn" },
            v => v * 0.514444, v => v / 0.514444),

        // ── AREA (base = square meter) ────────────────────────────────────────
        new("square meter",     "area", new[] { "m2", "m²", "sq m", "square meters", "square metres" },
            v => v, v => v),
        new("square kilometer", "area", new[] { "km2", "km²", "sq km", "square kilometers" },
            v => v * 1_000_000, v => v / 1_000_000),
        new("square foot",      "area", new[] { "ft2", "ft²", "sq ft", "square feet" },
            v => v * 0.092903, v => v / 0.092903),
        new("square inch",      "area", new[] { "in2", "in²", "sq in", "square inches" },
            v => v * 0.00064516, v => v / 0.00064516),
        new("acre",             "area", new[] { "acres" },
            v => v * 4046.86, v => v / 4046.86),
        new("hectare",          "area", new[] { "ha", "hectares" },
            v => v * 10_000, v => v / 10_000),
    };

    /// <summary>Finds a unit definition by canonical name or any alias (case-insensitive).</summary>
    public static UnitDefinition? Find(string nameOrAlias)
    {
        var key = nameOrAlias.Trim().ToLowerInvariant();
        return _units.FirstOrDefault(u =>
            u.Name.Equals(key, StringComparison.OrdinalIgnoreCase) ||
            u.Aliases.Any(a => a.Equals(key, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>Returns all known units, optionally filtered by category.</summary>
    public static IEnumerable<UnitDefinition> GetAll(string? category = null) =>
        category is null
            ? _units
            : _units.Where(u => u.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

    /// <summary>Returns all distinct category names.</summary>
    public static IEnumerable<string> GetCategories() =>
        _units.Select(u => u.Category).Distinct(StringComparer.OrdinalIgnoreCase);
}