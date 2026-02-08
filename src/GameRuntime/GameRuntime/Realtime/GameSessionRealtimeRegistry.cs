using System.Collections.Concurrent;

namespace GameRuntime.Realtime;

internal sealed class GameSessionRealtimeRegistry : IGameSessionRealtimeRegistry
{
    private readonly ConcurrentDictionary<Guid, GameSessionRealtimeState> _sessions = new();

    public GameSessionRealtimeState Get(Guid sessionId)
        => _sessions.GetOrAdd(sessionId, _ => new GameSessionRealtimeState());

    public void MarkClientConnected(Guid sessionId)
    {
        GameSessionRealtimeState state = Get(sessionId);
        Interlocked.Increment(ref state.ConnectedClients);
    }

    public void MarkClientDisconnected(Guid sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out GameSessionRealtimeState? state))
        {
            Interlocked.Decrement(ref state.ConnectedClients);
        }
    }

    public void Remove(Guid sessionId)
    {
        _sessions.TryRemove(sessionId, out _);
    }
}
