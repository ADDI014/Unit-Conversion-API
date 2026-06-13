using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversionApi.Models;
using UnitConversionApi.Services;
using Xunit;

namespace UnitConversionApi.Tests;

// ── Unit tests for ConversionService ────────────────────────────────────────
public class ConversionServiceTests
{
    private readonly IConversionService _svc = new ConversionService();

    [Theory]
    [InlineData(1,   "meter",   "foot",        3.28084)]
    [InlineData(100, "kilometer","mile",       62.1371)]
    [InlineData(1,   "mile",    "meter",     1609.344)]
    [InlineData(1,   "inch",    "centimeter",   2.54)]
    public void LengthConversions_AreCorrect(double value, string from, string to, double expected)
    {
        var result = _svc.Convert(new ConversionRequest { Value = value, FromUnit = from, ToUnit = to });
        Assert.NotNull(result);
        Assert.Equal(expected, result!.OutputValue, 3);
        Assert.Equal("length", result.Category);
    }

    [Theory]
    [InlineData(0,   "celsius",    "fahrenheit", 32)]
    [InlineData(100, "celsius",    "fahrenheit", 212)]
    [InlineData(32,  "fahrenheit", "celsius",    0)]
    [InlineData(0,   "celsius",    "kelvin",     273.15)]
    public void TemperatureConversions_AreCorrect(double value, string from, string to, double expected)
    {
        var result = _svc.Convert(new ConversionRequest { Value = value, FromUnit = from, ToUnit = to });
        Assert.NotNull(result);
        Assert.Equal(expected, result!.OutputValue, 2);
    }

    [Theory]
    [InlineData(1,  "kilogram", "pound",     2.20462)]
    [InlineData(1,  "pound",   "gram",     453.592)]
    [InlineData(1,  "tonne",   "kilogram", 1000)]
    public void WeightConversions_AreCorrect(double value, string from, string to, double expected)
    {
        var result = _svc.Convert(new ConversionRequest { Value = value, FromUnit = from, ToUnit = to });
        Assert.NotNull(result);
        Assert.Equal(expected, result!.OutputValue, 2);
    }

    [Fact]
    public void UnknownUnit_ReturnsNull()
    {
        var result = _svc.Convert(new ConversionRequest { Value = 1, FromUnit = "parsec", ToUnit = "meter" });
        Assert.Null(result);
    }

    [Fact]
    public void CrossCategoryConversion_ReturnsNull()
    {
        var result = _svc.Convert(new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "kilogram" });
        Assert.Null(result);
    }

    [Fact]
    public void SameUnit_ReturnsSameValue()
    {
        var result = _svc.Convert(new ConversionRequest { Value = 42, FromUnit = "meter", ToUnit = "m" });
        Assert.NotNull(result);
        Assert.Equal(42, result!.OutputValue, 5);
    }

    [Fact]
    public void AliasesWork()
    {
        // "ft" is an alias for "foot"
        var result = _svc.Convert(new ConversionRequest { Value = 1, FromUnit = "m", ToUnit = "ft" });
        Assert.NotNull(result);
        Assert.Equal(3.28084, result!.OutputValue, 3);
    }
}

// ── Integration tests for HTTP endpoints ─────────────────────────────────────
public class ConversionControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConversionControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_ValidConversion_Returns200()
    {
        var payload = new { value = 100, fromUnit = "celsius", toUnit = "fahrenheit" };
        var response = await _client.PostAsJsonAsync("/api/conversion", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ConversionResult>();
        Assert.NotNull(result);
        Assert.Equal(212, result!.OutputValue, 2);
    }

    [Fact]
    public async Task Post_UnknownUnit_Returns400()
    {
        var payload = new { value = 1, fromUnit = "parsec", toUnit = "meter" };
        var response = await _client.PostAsJsonAsync("/api/conversion", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_Categories_Returns200WithList()
    {
        var response = await _client.GetAsync("/api/conversion/categories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.Contains("length", categories!);
        Assert.Contains("temperature", categories!);
    }

    [Fact]
    public async Task Get_Units_FilteredByCategory_Returns200()
    {
        var response = await _client.GetAsync("/api/conversion/units?category=weight");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}