using System.Reflection;
using Application;
using Application.Abstractions.GameRuntime;
using Aspire.ServiceDefaults;
using Domain.ValueObjects;
using GameRuntime;
using GameRuntime.Logic.User.Api;
using GameRuntime.Realtime;
using GameRuntime.UserCodeApiDocumentation;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Web.Api;
using Web.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => builder.Environment.IsDevelopment());
        }
    });
});

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration)
    .AddGameRuntimeInfrastructure()
    .AddUserCodeApiDocumentation([typeof(UserWorldView).Assembly, typeof(Position).Assembly]);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    app.ApplyMigrations();
}

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Database:SeedOnStartup"))
{
    await app.DatabaseSeed();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();

app.UseExceptionHandler();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

// REMARK: If you want to use Controllers, you'll need this.
app.MapControllers();

app.MapHub<GameRuntimeHub>(GameRuntimeHubConstants.HubUrl);

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace Web.Api
{
    public partial class Program;
}
