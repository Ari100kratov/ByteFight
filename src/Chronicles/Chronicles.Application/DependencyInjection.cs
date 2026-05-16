using Chronicles.Application.Chronicles.GetCharacterChronicles;
using Chronicles.Application.Chronicles.GetTopNominations;
using Chronicles.Application.Chronicles.ProcessCompletedGameSessions;
using Chronicles.Application.Chronicles.ProcessCompletedGameSessions.Nominations;
using Chronicles.Application.Chronicles.RebuildProjections;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Chronicles.Application;

/// <summary>
/// Регистрация сервисов application-слоя подсистемы хроник.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Добавляет сервисы application-слоя хроник.
    /// </summary>
    public static IServiceCollection AddChroniclesApplication(this IServiceCollection services)
    {
        services.AddScoped<IChroniclesProjectionService, ChroniclesProjectionService>();
        services.AddScoped<ChroniclesProjectionSessionProcessor>();
        services.AddScoped<ICharacterChroniclesReader, CharacterChroniclesReader>();
        services.AddScoped<IChroniclesLeaderboardReader, ChroniclesLeaderboardReader>();
        services.AddScoped<IChroniclesReprojectionService, ChroniclesReprojectionService>();

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IChronicleNominationProjector)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
