using Domain.Game.Abilities;
using Domain.Game.Stats;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.ValueObjects;
using GameRuntime.Common;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Combat;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.Actions;
using GameRuntime.Logic.NPC.PathFinding;
using GameRuntime.Logic.Turns;

namespace GameRuntime.Logic.NPC;

/// <summary>
/// Тактический ИИ врага: тратит очки действия и перемещения по смыслу —
/// лечит раненых соратников, выбирает сильнейшую доступную способность
/// и держит дистанцию, если бить издалека выгоднее.
/// </summary>
internal sealed class TacticalEnemyAiRunner(IPathFinder pathFinder) : IBattleTurnRunner
{
    public async Task<IReadOnlyList<GameActionLogEntry>> PlayTurn(
        BaseUnit unit,
        ArenaWorld world,
        Action<IReadOnlyList<GameActionLogEntry>> onActionPerformed,
        CancellationToken ct)
    {
        await Task.CompletedTask;

        var logs = new List<GameActionLogEntry>();

        if (unit is not EnemyUnit enemy)
        {
            throw new InvalidOperationException(
                $"{nameof(TacticalEnemyAiRunner)} может управлять только врагами.");
        }

        // Защитные рефлексы: самовосстановление при низком здоровье.
        logs.AddRange(TryEmergencyHeal(enemy, world, onActionPerformed));

        // До двух действий за ход.
        for (int action = 0; action < UnitBattleState.ActionsPerTurn; action++)
        {
            if (enemy.IsDead || world.Player.IsDead || enemy.BattleState.ActionsRemaining <= 0)
            {
                break;
            }

            List<GameActionLogEntry> step = PerformBestAction(enemy, world);

            if (step.Count == 0)
            {
                break;
            }

            logs.AddRange(step);
            onActionPerformed(step);
        }

        // Перемещение после действий, если ноги ещё держат.
        logs.AddRange(TryMove(enemy, world, onActionPerformed));

        return logs;
    }

    /// <summary>
    /// Разовое самовосстановление, когда здоровье падает ниже трети.
    /// </summary>
    private IReadOnlyList<GameActionLogEntry> TryEmergencyHeal(
        EnemyUnit enemy,
        ArenaWorld world,
        Action<IReadOnlyList<GameActionLogEntry>> onActionPerformed)
    {
        if (enemy.Stats.GetHealthPercent() >= 0.33m || enemy.AiState.HasSelfHealedAfterBeingHit)
        {
            return [];
        }

        RuntimeAbility? heal = enemy.Abilities.All.FirstOrDefault(a =>
            a.EffectType == AbilityEffectType.Healing &&
            a.Get(AbilityStatType.Healing) > 0 &&
            a.TargetType == AbilityTargetType.Self &&
            enemy.BattleState.GetCooldown(a.Type) == 0);

        if (heal is null || !enemy.Stats.TrySpendMana(heal.GetManaCost()))
        {
            return [];
        }

        enemy.AiState.MarkSelfHealedAfterBeingHit();

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(enemy, heal, enemy.Position).Execute(world).ToList();

        onActionPerformed(logs);

        return logs;
    }

    /// <summary>
    /// Выбирает и выполняет лучшее действие: сильнейшая доступная
    /// способность по игроку с учётом дистанции и перезарядок.
    /// </summary>
    private List<GameActionLogEntry> PerformBestAction(EnemyUnit enemy, ArenaWorld world)
    {
        BaseUnit target = world.Player;

        if (target.IsDead)
        {
            return [];
        }

        int distance = enemy.Position.HexDistance(target.Position);

        RuntimeAbility? best = enemy.Abilities.All
            .Where(a => a.EffectType is AbilityEffectType.Damage or AbilityEffectType.Debuff)
            .Where(a => a.Get(AbilityStatType.Damage) > 0)
            .Where(a => enemy.BattleState.GetCooldown(a.Type) == 0)
            .Where(a => a.GetManaCost() <= enemy.Stats.Get(StatType.Mana))
            .Where(a => a.CanReach(distance))
            .Where(a => TargetingRules.GetTargetHexes(world, enemy, a).Count > 0)
            .OrderByDescending(a => a.Get(AbilityStatType.Damage))
            .FirstOrDefault();

        if (best is null)
        {
            return [];
        }

        // Для способностей по области целимся прямо в игрока,
        // для одиночных — тоже: он стоит на этом гексе.
        return new UseAbilityAction(enemy, best, target.Position).Execute(world).ToList();
    }

    /// <summary>
    /// Сближение с игроком по пути, укладывающемуся в очки перемещения.
    /// Дальнобойные враги наоборот держат дистанцию.
    /// </summary>
    private IReadOnlyList<GameActionLogEntry> TryMove(
        EnemyUnit enemy,
        ArenaWorld world,
        Action<IReadOnlyList<GameActionLogEntry>> onActionPerformed)
    {
        if (enemy.IsDead ||
            world.Player.IsDead ||
            enemy.BattleState.MovePointsRemaining <= 0 ||
            enemy.BattleState.Statuses.IsRooted)
        {
            return [];
        }

        System.Collections.Immutable.ImmutableDictionary<Position, int> costs =
            MovementRules.GetReachableCellCosts(
                world,
                enemy,
                enemy.BattleState.MovePointsRemaining);

        bool isRanged = enemy.Abilities.All
            .Where(a => a.EffectType == AbilityEffectType.Damage)
            .Select(a => a.GetRange())
            .DefaultIfEmpty(0)
            .Max() > 1;

        Position target = world.Player.Position;

        // Ближний бой: минимизировать дистанцию до игрока.
        // Дальний бой: держаться в трёх гексах от цели.
        int Score(Position cell) => isRanged
            ? -Math.Abs(cell.HexDistance(target) - 3)
            : -cell.HexDistance(target);

        Position? bestCell = costs
            .Where(kv => kv.Key != enemy.Position)
            .Where(kv => kv.Value <= enemy.BattleState.MovePointsRemaining)
            .OrderByDescending(kv => Score(kv.Key))
            .ThenBy(kv => kv.Value)
            .Select(kv => kv.Key)
            .FirstOrDefault();

        if (bestCell is null)
        {
            return [];
        }

        List<Position>? path = pathFinder.FindPath(world, enemy.Position, bestCell);

        if (path is null || path.Count < 2)
        {
            return [];
        }

        IReadOnlyList<GameActionLogEntry> logs =
            new MoveAction(enemy, path).Execute(world).ToList();

        onActionPerformed(logs);

        return logs;
    }
}
