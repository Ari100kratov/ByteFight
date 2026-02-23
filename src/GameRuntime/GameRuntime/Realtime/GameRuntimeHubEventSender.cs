using Application.Abstractions.GameRuntime;
using Application.Contracts.GameRuntime;
using Domain.GameRuntime.GameActionLogs;
using Domain.GameRuntime.GameSessions;
using Microsoft.AspNetCore.SignalR;

namespace GameRuntime.Realtime;

internal sealed class GameRuntimeHubEventSender(
    IHubContext<GameRuntimeHub> hub,
    IGameSessionRealtimeRegistry registry) : IGameRuntimeEventSender
{
    public async Task SendTick(Guid gameSessionId, TurnLog log, CancellationToken ct)
    {
        GameSessionRealtimeState state = registry.Get(gameSessionId);

        if (!state.HasClients)
        {
            state.PendingLogs.Enqueue(log);
            return;
        }

        // Сначала отправляем накопленные
        while (state.PendingLogs.TryDequeue(out TurnLog? pending))
        {
            await hub.Clients
                .Group(gameSessionId.ToString())
                .SendAsync(GameRuntimeEvents.Tick, pending.ToDto(), ct);
        }

        // Потом текущий тик
        await hub.Clients
            .Group(gameSessionId.ToString())
            .SendAsync(GameRuntimeEvents.Tick, log.ToDto(), ct);
    }

    public async Task SendFinished(GameSession session, CancellationToken ct)
    {
        registry.Remove(session.Id);

        await hub.Clients
            .Group(session.Id.ToString())
            .SendAsync(GameRuntimeEvents.Finished, session.ToDto(), ct);
    }
}
