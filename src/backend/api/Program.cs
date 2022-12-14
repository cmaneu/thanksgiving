// TODO: Proper rename async methods
using api.Adapters;
using api.Endpoints;
using api.Services;
using api.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddLogging();

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
builder.Services.AddFeatureManagement();

builder.Services.AddDbContext<ThanksDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration["THANKS_DB_CONNECTION"]);
}
);

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection(nameof(AuthTokenSettings))["JwtSigningKey"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetSection(nameof(AuthTokenSettings))["DefaultIssuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetSection(nameof(AuthTokenSettings))["APIAudience"],
    };
});

builder.Services.AddAuthorizationBuilder().AddCurrentUserHandler();

builder.Services.AddCurrentUser();

builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IAuthenticationTokenStore, InMemoryAuthenticationTokenStore>();

builder.Services.AddScoped<IUserRepository, FakeUserRepository>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<IOrganizationRepository, EFOrganizationRepositoryAdapter>();

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(15)));
    //options.AddPolicy("Expire30", builder => builder.Expire(TimeSpan.FromSeconds(30)));
});

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

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapAuthEndpoints();
app.MapOrganizationActivityEndpoints();

app.MapGet("/me", (CurrentIdentity id) => new { id.User.FirstName }).RequireAuthorization();

app.MapGet("/organizations", async (OrganizationService orgService) => { return await orgService.GetAllPublicOrganizationsAsync(); }).CacheOutput();

app.Run();
