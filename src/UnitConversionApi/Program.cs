using UnitConversionApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IConversionService, ConversionService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Unit Conversion API",
        Version = "v1",
        Description = "Converts numerical values between units of measurement."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Conversion API v1");
    c.RoutePrefix = "swagger";
});

// ❌ Remove this — Render handles HTTPS, this causes redirect loops
// app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

public partial class Program { }