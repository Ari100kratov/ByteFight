using Domain.Game.Abilities;
using Domain.GameRuntime.GameActionLogs.Entries;
using GameRuntime.Common.World.Stats;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Combat;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Logic.Actions;

/// <summary>
/// Применение способности: проверка ресурсов, дальности и формы области,
/// расчёт эффектов по каждой поражённой цели, наложение статусов
/// и запуск перезарядки.
/// </summary>
internal sealed class UseAbilityAction : IRuntimeAction
{
    private readonly BaseUnit actor;
    private readonly RuntimeAbility ability;
    private readonly Position targetHex;

    public UseAbilityAction(
        BaseUnit actor,
        RuntimeAbility ability,
        Position targetHex)
    {
        this.actor = actor;
        this.ability = ability;
        this.targetHex = targetHex;
    }

    public IEnumerable<GameActionLogEntry> Execute(ArenaWorld world)
    {
        UnitBattleState state = actor.BattleState;

        if (actor.IsDead)
        {
            yield return world.CreateIdleLogEntry(actor, IdleReasons.InvalidAction);
            yield break;
        }

        if (state.ActionsRemaining < ability.GetActionCost())
        {
            yield return world.CreateIdleLogEntry(actor, IdleReasons.NoActionPoints);
            yield break;
        }

        if (state.GetCooldown(ability.Type) > 0)
        {
            yield return world.CreateIdleLogEntry(actor, IdleReasons.OnCooldown);
            yield break;
        }

        if (ability.GetManaCost() > 0 && !actor.Stats.TrySpendMana(ability.GetManaCost()))
        {
            yield return world.CreateIdleLogEntry(actor, IdleReasons.NotEnoughMana);
            yield break;
        }

        int distance = actor.Position.HexDistance(targetHex);

        if (!ability.CanReach(distance))
        {
            yield return world.CreateIdleLogEntry(actor, IdleReasons.OutOfRange);
            yield break;
        }

        IReadOnlyList<Position> validTargets = TargetingRules.GetTargetHexes(world, actor, ability);

        if (!validTargets.Contains(targetHex))
        {
            yield return world.CreateIdleLogEntry(actor, IdleReasons.InvalidTarget);
            yield break;
        }

        // Списание ресурсов и поворот к цели.
        state.ActionsRemaining -= ability.GetActionCost();
        state.StartCooldown(ability.Type, ability.GetCooldown());
        actor.FaceTarget(targetHex);

        // Рывок: применяющий перемещается к цели.
        if (ability.DashesToTarget && distance > 1)
        {
            foreach (GameActionLogEntry entry in ExecuteDash(world, targetHex))
            {
                yield return entry;
            }
        }

        // Поражаемые гексы по форме способности.
        IReadOnlyList<Position> affectedHexes =
            TargetingRules.GetAffectedHexes(world, actor, ability, targetHex);

        // Основной эффект — по всем юнитам в области (кроме применяющего),
        // кроме способностей на себя.
        IEnumerable<BaseUnit> affectedUnits = ability.TargetType == AbilityTargetType.Self
            ? [actor]
            : affectedHexes
                .Select(world.GetOccupant)
                .Where(u => u is not null && u != actor)
                .DistinctBy(u => u!.Id)
                .Cast<BaseUnit>();

        bool anyEffect = false;

        foreach (BaseUnit target in affectedUnits)
        {
            if (ability.EffectType is AbilityEffectType.Damage or AbilityEffectType.Debuff &&
                !world.IsHostile(actor, target))
            {
                // Повреждающие эффекты не задевают соратников.
                continue;
            }

            foreach (GameActionLogEntry entry in ApplyToTarget(world, target))
            {
                anyEffect = true;
                yield return entry;
            }
        }

        if (!anyEffect)
        {
            yield return world.CreateIdleLogEntry(actor, IdleReasons.NoTargetsHit);
        }
    }

    private IEnumerable<GameActionLogEntry> ApplyToTarget(ArenaWorld world, BaseUnit target)
    {
        bool affected = false;

        switch (ability.EffectType)
        {
            case AbilityEffectType.Damage:
            {
                decimal rawDamage = DamageRules.CalculateOutgoingDamage(actor, ability);

                if (rawDamage > 0)
                {
                    decimal applied = DamageRules.ApplyIncomingDamage(world, target, rawDamage);

                    affected = true;

                    yield return world.CreateAbilityUsedLogEntry(
                        actor,
                        target,
                        ability,
                        applied,
                        new Domain.ValueObjects.StatSnapshot(
                            target.Stats.GetHealth(),
                            target.Stats.GetMaxHealth()));

                    if (target.IsDead)
                    {
                        target.MarkKilledBy(actor.Id);
                        yield return world.CreateDeathLogEntry(target);
                    }
                }

                break;
            }

            case AbilityEffectType.Healing:
            {
                decimal healing = DamageRules.CalculateOutgoingHealing(actor, ability);

                if (healing > 0)
                {
                    StatApplyResult result = target.Stats.Heal(healing);

                    affected = true;

                    yield return world.CreateAbilityUsedLogEntry(
                        actor,
                        target,
                        ability,
                        result.AppliedValue,
                        result.Snapshot);
                }

                break;
            }
        }

        // Статус-эффекты накладываются независимо от основного эффекта:
        // ядовитая стрела бьёт и отравляет, оберег — щит плюс регенерация.
        foreach (AbilityStatusEffect status in ability.AppliedStatuses)
        {
            affected = true;

            target.BattleState.Statuses.Apply(
                status.Type,
                status.Duration,
                status.Magnitude);

            yield return world.CreateStatusAppliedLogEntry(
                actor,
                target,
                status.Type,
                status.Duration,
                status.Magnitude,
                new Domain.ValueObjects.StatSnapshot(
                    target.Stats.GetHealth(),
                    target.Stats.GetMaxHealth()));
        }

        if (!affected)
        {
            yield return world.CreateIdleLogEntry(actor, IdleReasons.InvalidAction);
        }
    }

    /// <summary>
    /// Богатырский рывок: перемещение по прямой к гексу рядом с целью.
    /// </summary>
    private IEnumerable<GameActionLogEntry> ExecuteDash(ArenaWorld world, Position target)
    {
        List<Position> line = HexGeometry.GetLine(actor.Position, target);

        Position? landing = null;

        for (int i = line.Count - 2; i >= 1; i--)
        {
            if (GameRuntime.Common.MovementRules.CanStandOn(world, actor, line[i]))
            {
                landing = line[i];
                break;
            }
        }

        if (landing is null || landing == actor.Position)
        {
            yield break;
        }

        List<Position> path = [.. line.TakeWhile(p => p != landing), landing];

        foreach (Position step in path.Skip(1))
        {
            actor.MoveStep(step);
        }

        yield return world.CreateWalkLogEntry(actor, path);
    }
}
