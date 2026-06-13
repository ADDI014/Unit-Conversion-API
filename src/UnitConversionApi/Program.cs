using UnitConversionApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IConversionService, ConversionService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Unit Conversion API",
        Version     = "v1",
        Description = "Converts numerical values between units of measurement."
    });
});

var app = builder.Build();

// ✅ Always enable Swagger (not just in Development)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Conversion API v1");
    c.RoutePrefix = "swagger"; // ← change from empty string to "swagger"
});

// ✅ Remove UseHttpsRedirection — Render handles HTTPS externally
// app.UseHttpsRedirection();  ← DELETE or comment this line

app.UseAuthorization();
app.MapControllers();

// ✅ Add a root redirect so "/" goes to swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

public partial class Program { }