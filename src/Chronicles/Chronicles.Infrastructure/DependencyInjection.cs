using Chronicles.Application.Abstractions.Data;
using Chronicles.Application.Chronicles.ImportInbox;
using Chronicles.Application.Chronicles.ProcessCompletedGameSessions;
using Chronicles.Infrastructure.Database;
using Chronicles.Infrastructure.Outbox;
using Chronicles.Infrastructure.Source.GameRuntime;
using Chronicles.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedKernel;

namespace Chronicles.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddChroniclesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddChroniclesPersistence(configuration)
            .AddChroniclesGameRuntimeOutboxImporter(configuration);

    public static IServiceCollection AddChroniclesPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string chroniclesConnectionString =
            configuration.GetConnectionString("ChroniclesDatabase")
            ?? configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'ChroniclesDatabase' or 'Database' is required.");

        services.AddDbContext<ChroniclesDbContext>(options => options
            .UseNpgsql(chroniclesConnectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Chronicles))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IChroniclesDbContext>(sp => sp.GetRequiredService<ChroniclesDbContext>());
        services.AddScoped<ICompletedGameSessionPayloadParser, GameSessionCompletedPayloadParser>();
        services.TryAddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }

    public static IServiceCollection AddChroniclesGameRuntimeOutboxImporter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string sourceConnectionString =
            configuration.GetConnectionString("GameRuntimeDatabase")
            ?? configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'GameRuntimeDatabase' or 'Database' is required.");

        services.AddDbContext<SourceGameRuntimeReadDbContext>(options => options
            .UseNpgsql(sourceConnectionString)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IChroniclesInboxImportService, ChroniclesInboxImportService>();

        return services;
    }
}
