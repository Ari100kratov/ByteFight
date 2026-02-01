using Microsoft.AspNetCore.SignalR;

namespace GameRuntime.Realtime;

public sealed class GameRuntimeHub(IGameSessionRealtimeRegistry registry) : Hub
{
    public async Task JoinGame(Guid gameSessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameSessionId.ToString());
        registry.MarkClientConnected(gameSessionId);
    }

    public async Task LeaveGame(Guid gameSessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameSessionId.ToString());
        registry.MarkClientDisconnected(gameSessionId);
    }
}
