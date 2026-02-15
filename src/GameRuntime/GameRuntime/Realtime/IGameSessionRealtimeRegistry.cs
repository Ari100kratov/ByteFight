using System.Collections.Concurrent;
using Domain.GameRuntime.RuntimeLogEntries;

namespace GameRuntime.Realtime;

public interface IGameSessionRealtimeRegistry
{
    GameSessionRealtimeState Get(Guid sessionId);
    void MarkClientConnected(Guid sessionId);
    void MarkClientDisconnected(Guid sessionId);
    void Remove(Guid sessionId);
}

public sealed class GameSessionRealtimeState
{
#pragma warning disable S1104 // Fields should not have public accessibility
    public volatile int ConnectedClients;
#pragma warning restore S1104 // Fields should not have public accessibility
    public ConcurrentQueue<TurnLog> PendingLogs { get; } = new();

    public bool HasClients => ConnectedClients > 0;
}
