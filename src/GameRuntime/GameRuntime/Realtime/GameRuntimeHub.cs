using Application.Contracts;
using Domain.Game.Abilities;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using Microsoft.AspNetCore.SignalR;

namespace GameRuntime.Realtime;

/// <summary>
/// SignalR-хаб боя: подписка на события сессии и команды игрока
/// в ручном режиме.
/// </summary>
public sealed class GameRuntimeHub(
    IGameSessionRealtimeRegistry registry,
    BattleCommandRegistry commands) : Hub
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

    /// <summary>
    /// Команда перемещения по пути гексов.
    /// </summary>
    public Task SubmitMove(Guid gameSessionId, PositionDto[] path)
    {
        List<Position> positions =
        [
            .. path.Select(p => new Position(p.X, p.Y))
        ];

        return TryDispatch(gameSessionId, new MoveCommand(positions));
    }

    /// <summary>
    /// Команда применения способности на целевой гекс.
    /// </summary>
    public Task SubmitAbility(Guid gameSessionId, int abilityType, int targetX, int targetY)
    {
        if (!Enum.IsDefined(typeof(AbilityType), abilityType))
        {
            return Task.CompletedTask;
        }

        return TryDispatch(
            gameSessionId,
            new UseAbilityCommand(
                (AbilityType)abilityType,
                new Position(targetX, targetY)));
    }

    /// <summary>
    /// Команда завершения хода.
    /// </summary>
    public Task SubmitEndTurn(Guid gameSessionId) =>
        TryDispatch(gameSessionId, new EndTurnCommand());

    private Task TryDispatch(Guid gameSessionId, PlayerBattleCommand command)
    {
        commands.TryPost(gameSessionId, command);

        return Task.CompletedTask;
    }
}
