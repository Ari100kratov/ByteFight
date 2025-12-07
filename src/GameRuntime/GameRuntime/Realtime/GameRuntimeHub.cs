using Microsoft.AspNetCore.SignalR;

namespace GameRuntime.Realtime;

public sealed class GameRuntimeHub : Hub
{
    public Task JoinGame(Guid gameSessionId)
        => Groups.AddToGroupAsync(Context.ConnectionId, gameSessionId.ToString());

    public Task LeaveGame(Guid gameSessionId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, gameSessionId.ToString());
}
