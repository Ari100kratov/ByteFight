using Application.Abstractions.GameRuntime;
using GameRuntime.Builders;
using GameRuntime.Hosting;
using GameRuntime.Logic.NPC;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.User.Compilation;
using GameRuntime.Logic.User.Execution;
using GameRuntime.Logic.User.Intellisense.Services;
using GameRuntime.Logic.User.Intellisense.Workspace;
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

        // Compilation
        services.AddSingleton<UserActionExecutor>();
        services.AddSingleton<UserScriptCompiler>();

        // Intellisense
        services.AddSingleton<UserScriptWorkspace>();
        services.AddSingleton<UserScriptCompletionService>();
        services.AddSingleton<UserScriptDiagnosticsService>();
        services.AddSingleton<UserScriptHoverService>();
        services.AddSingleton<UserScriptSignatureHelpService>();

        services.AddSingleton<IGameSessionRepository, GameSessionRepository>();
        services.AddSingleton<IPathFinder, PathFinder>();

        services.AddSignalR();
        services.AddSingleton<IGameSessionRealtimeRegistry, GameSessionRealtimeRegistry>();
        services.AddSingleton<IGameRuntimeEventSender, GameRuntimeHubEventSender>();

        return services;
    }
}
