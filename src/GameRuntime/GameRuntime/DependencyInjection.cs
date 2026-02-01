using Application.Abstractions.GameRuntime;
using GameRuntime.Builders;
using GameRuntime.Hosting;
using GameRuntime.Logic.NPC;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.User.Compilation;
using GameRuntime.Logic.User.Execution;
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

        services.AddSingleton<BasicEnemyAiProcessor>();
        services.AddSingleton<UserActionExecutor>();
        services.AddSingleton<UserScriptCompiler>();

        services.AddSingleton<IGameSessionRepository, GameSessionRepository>();
        services.AddSingleton<IPathFinder, PathFinder>();

        services.AddSignalR();
        services.AddSingleton<IGameSessionRealtimeRegistry, GameSessionRealtimeRegistry>();
        services.AddSingleton<IGameRuntimeEventSender, GameRuntimeHubEventSender>();

        return services;
    }
}
