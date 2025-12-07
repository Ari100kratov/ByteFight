using Domain.GameRuntime.GameSessions;
using Domain.GameRuntime.RuntimeLogEntries;

namespace Application.Abstractions.GameRuntime;

public interface IGameRuntimeEventSender
{
    Task SendTick(Guid gameSessionId, TurnLog log, CancellationToken ct);
    Task SendFinished(GameSession session, CancellationToken ct);
}

public static class GameRuntimeEvents
{
    public const string Tick = "Tick";
    public const string Finished = "Finished";
}

public static class GameRuntimeHubConstants
{
    public const string HubUrl = "/game-runtime-hub";
}
