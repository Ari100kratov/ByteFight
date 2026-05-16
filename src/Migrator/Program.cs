using Application;
using Aspire.ServiceDefaults;
using Chronicles.Application;
using Chronicles.Application.Chronicles.RebuildProjections;
using Chronicles.Infrastructure;
using GameRuntime.Integration;
using Infrastructure;
using Infrastructure.Database.Auth;
using Infrastructure.Database.Game;
using Infrastructure.Database.GameRuntime;
using Infrastructure.Database.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Migrator;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddOptions<MigratorOptions>()
    .Bind(builder.Configuration.GetSection(MigratorOptions.SectionName))
    .Validate(
        options => options.ChroniclesCatchUpBatchSize > 0,
        "Migrator:ChroniclesCatchUpBatchSize must be greater than zero.");

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddChroniclesApplication()
    .AddChroniclesInfrastructure(builder.Configuration)
    .AddScoped<IGameSessionCompletedIntegrationEventFactory, GameSessionCompletedIntegrationEventFactory>()
    .AddScoped<CompletedSessionsOutboxBackfillService>()
    .AddScoped<ChroniclesCatchUpService>()
    .AddScoped<ChroniclesNominationMetadataBackfillService>();

using IHost host = builder.Build();

await using AsyncServiceScope scope = host.Services.CreateAsyncScope();

IServiceProvider services = scope.ServiceProvider;
MigratorOptions migratorOptions = services.GetRequiredService<IOptions<MigratorOptions>>().Value;
bool rebuildChronicles = migratorOptions.RebuildChroniclesProjections ||
    args.Any(x => string.Equals(x, "--rebuild-chronicles", StringComparison.OrdinalIgnoreCase));

await services.GetRequiredService<AuthDbContext>().Database.MigrateAsync();
await services.GetRequiredService<GameDbContext>().Database.MigrateAsync();
await services.GetRequiredService<GameRuntimeDbContext>().Database.MigrateAsync();
await services.GetRequiredService<Chronicles.Infrastructure.Database.ChroniclesDbContext>().Database.MigrateAsync();

await services.GetRequiredService<DatabaseSeeder>().Seed();
await services.GetRequiredService<CompletedSessionsOutboxBackfillService>().BackfillAsync();
await services.GetRequiredService<ChroniclesCatchUpService>().CatchUpAsync(
    migratorOptions.ChroniclesCatchUpBatchSize);

if (rebuildChronicles)
{
    ILogger logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("Migrator");
    MigratorLogMessages.ChroniclesRebuildRequested(logger);

    await services.GetRequiredService<IChroniclesReprojectionService>().RebuildAsync();
}

await services.GetRequiredService<ChroniclesNominationMetadataBackfillService>().BackfillAsync();
