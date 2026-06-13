using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public ConversionController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/conversion
    ///     {
    ///         "value": 100,
    ///         "fromUnit": "celsius",
    ///         "toUnit": "fahrenheit"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Conversion successful.</response>
    /// <response code="400">Unknown unit or cross-category conversion attempted.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ConversionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ConversionResult> Convert([FromBody] ConversionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FromUnit) || string.IsNullOrWhiteSpace(request.ToUnit))
            return BadRequest(new { error = "Both 'fromUnit' and 'toUnit' must be provided." });

        var result = _conversionService.Convert(request);

        if (result is null)
            return BadRequest(new
            {
                error = $"Cannot convert '{request.FromUnit}' to '{request.ToUnit}'. " +
                        "Check that both units exist and belong to the same category."
            });

        return Ok(result);
    }

    /// <summary>
    /// Returns all supported unit categories.
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<string>> GetCategories() =>
        Ok(_conversionService.GetCategories());

    /// <summary>
    /// Returns all supported units, optionally filtered by category.
    /// </summary>
    /// <param name="category">Optional category filter (e.g. "length", "temperature", "weight").</param>
    [HttpGet("units")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetUnits([FromQuery] string? category = null) =>
        Ok(_conversionService.GetUnits(category));
}