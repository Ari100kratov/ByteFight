using Application.Abstractions.GameRuntime;
using GameRuntime.Builders;
using GameRuntime.Hosting;
using GameRuntime.Logic.NPC;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.Turns;
using GameRuntime.Persistence;
using GameRuntime.Realtime;
using Microsoft.Extensions.DependencyInjection;

namespace GameRuntime;

public static class DependencyInjection
{
    public static IServiceCollection AddGameRuntimeInfrastructure(this IServiceCollection services) =>
        services.AddServices();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IGameHost, GameHost>();

        services.AddSingleton<ArenaWorldBuilder>();
        services.AddSingleton<GameInstanceFactory>();

        services.AddSingleton<IGameSessionRepository, GameSessionRepository>();
        services.AddSingleton<IGameTurnProcessor, GameTurnProcessor>();
        services.AddSingleton<IUnitTurnProcessor, BasicEnemyAiProcessor>();
        services.AddSingleton<IPathFinder, PathFinder>();

        services.AddSignalR();
        services.AddSingleton<IGameRuntimeEventSender, GameRuntimeHubEventSender>();

        return services;
    }
}
