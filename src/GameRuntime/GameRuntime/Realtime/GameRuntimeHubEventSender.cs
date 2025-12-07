using Application.Abstractions.GameRuntime;
using Application.Contracts.GameRuntime;
using Domain.GameRuntime.GameSessions;
using Domain.GameRuntime.RuntimeLogEntries;
using Microsoft.AspNetCore.SignalR;

namespace GameRuntime.Realtime;

internal sealed class GameRuntimeHubEventSender : IGameRuntimeEventSender
{
    private readonly IHubContext<GameRuntimeHub> _hub;

    public GameRuntimeHubEventSender(IHubContext<GameRuntimeHub> hub)
    {
        _hub = hub;
    }

    public Task SendTick(Guid gameSessionId, TurnLog log, CancellationToken ct)
        => _hub.Clients.Group(gameSessionId.ToString())
            .SendAsync(GameRuntimeEvents.Tick, log.ToDto(), ct);

    public Task SendFinished(GameSession session, CancellationToken ct)
        => _hub.Clients.Group(session.Id.ToString())
            .SendAsync(GameRuntimeEvents.Finished, session.ToDto(), ct);
}
