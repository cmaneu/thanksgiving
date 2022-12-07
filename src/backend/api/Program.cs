using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Thanksgiving app API",
        Description = "The backend API for Thanksgiving app",
        TermsOfService = new Uri("https://github.com/cmaneu/thanksgiving"),
        Contact = new OpenApiContact
        {
            Name = "GitHub discussions",
            Url = new Uri("https://github.com/cmaneu/thanksgiving/discussions")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://github.com/cmaneu/thanksgiving/blob/main/LICENSE")
        }
    });

    options.InferSecuritySchemes();
    options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer() { Url = "http://localhost:5029", Description = "Localhost unsecure" });
    options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer() { Url = "https://localhost:7294", Description = "Localhost HTTPS" });
    options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer() { Url = "http://localhost:8080", Description = "Localhost Docker" });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(bool.Parse(builder.Configuration["THANKS_DISABLE_HTTPS_REDIRECTION"]?.ToString() ?? "false") == true)
{
    app.Services.GetService<ILoggerFactory>().CreateLogger("API Host").LogInformation("Disabling HTTPS Redirection");
    app.UseHttpsRedirection();
}

// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-7.0
app.MapHealthChecks("/.core/healthcheck");



var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
