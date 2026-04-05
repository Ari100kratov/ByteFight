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
using Microsoft.Extensions.Logging;

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
        services.AddSingleton(new UserCodeExecutionOptions { Timeout = TimeSpan.FromSeconds(3) });
        services.AddSingleton<IUserCodeRunner>(sp =>
        {
            ILogger<ProcessUserCodeRunner> logger = sp.GetRequiredService<ILogger<ProcessUserCodeRunner>>();

            string workerExePath = Path.Combine(
                AppContext.BaseDirectory,
                "user-code-worker",
                "GameRuntime.UserCodeWorker.exe");

            return new ProcessUserCodeRunner(workerExePath, logger);
        });


        // Intellisense
        services.AddSingleton<UserScriptRoslynContextFactory>();

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
