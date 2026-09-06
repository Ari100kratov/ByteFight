using Domain.GameRuntime.GameActionLogs.Entries;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Units;
using GameRuntime.Realtime;
using GameRuntime.Logic.Actions;

namespace GameRuntime.Logic.Turns;

/// <summary>
/// Ручное управление игроком: ход ждёт команд из очереди,
/// пока игрок не завершит ход, не израсходует всё
/// или бой не закончится.
/// </summary>
internal sealed class ManualPlayerTurnRunner(BattleCommandQueue commandQueue) : IBattleTurnRunner
{
    public async Task<IReadOnlyList<GameActionLogEntry>> PlayTurn(
        BaseUnit unit,
        ArenaWorld world,
        Action<IReadOnlyList<GameActionLogEntry>> onActionPerformed,
        CancellationToken ct)
    {
        var logs = new List<GameActionLogEntry>();

        while (!unit.IsDead && !ct.IsCancellationRequested)
        {
            // Бой закончился — ход мгновенно завершается.
            if (world.CheckGameOver() is not null)
            {
                break;
            }

            UnitBattleState state = unit.BattleState;

            bool canMove = state.MovePointsRemaining > 0 && !state.Statuses.IsRooted;
            bool canAct = state.ActionsRemaining > 0;

            if (!canMove && !canAct)
            {
                break;
            }

            PlayerBattleCommand? command = await commandQueue.ReadAsync(ct);

            if (command is null)
            {
                // Канал закрыт (бой отменён) — завершаем ход.
                break;
            }

            List<GameActionLogEntry> applied = ApplyCommand(unit, world, command, canMove);

            if (applied.Count > 0)
            {
                logs.AddRange(applied);
                onActionPerformed(applied);
            }
        }

        return logs;
    }

    private List<GameActionLogEntry> ApplyCommand(
        BaseUnit unit,
        ArenaWorld world,
        PlayerBattleCommand command,
        bool canMove)
    {
        switch (command)
        {
            case EndTurnCommand:
                unit.BattleState.ActionsRemaining = 0;
                unit.BattleState.MovePointsRemaining = 0;
                return [];

            case MoveCommand move:
            {
                if (!canMove)
                {
                    return [world.CreateIdleLogEntry(unit, IdleReasons.MoveImpossible)];
                }

                return new MoveAction(unit, move.Path).Execute(world).ToList();
            }

            case UseAbilityCommand use:
            {
                if (!unit.Abilities.TryGet(use.Ability, out RuntimeAbility? ability))
                {
                    return [world.CreateIdleLogEntry(unit, IdleReasons.InvalidAction)];
                }

                return new UseAbilityAction(unit, ability, use.TargetHex).Execute(world).ToList();
            }

            default:
                return [world.CreateIdleLogEntry(unit, IdleReasons.InvalidAction)];
        }
    }
}
