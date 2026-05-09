using Domain.Game.Abilities;
using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Logic.Actions;

internal sealed class UseAbilityAction
{
    private readonly BaseUnit actor;
    private readonly BaseUnit target;
    private readonly RuntimeAbility ability;

    public UseAbilityAction(
        BaseUnit actor,
        BaseUnit target,
        RuntimeAbility ability)
    {
        this.actor = actor;
        this.target = target;
        this.ability = ability;
    }

    public IReadOnlyList<GameActionLogEntry> Execute(ArenaWorld world)
    {
        if (actor.IsDead)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.InvalidAction)];
        }

        if (target.IsDead)
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.TargetDead)];
        }

        int distance = actor.Position.ManhattanDistance(target.Position);

        if (!ability.CanReach(distance))
        {
            return [world.CreateIdleLogEntry(actor, IdleReasons.OutOfRange)];
        }

        actor.Turn(GetFacingDirection(actor.Position, target.Position));

        return ability.EffectType switch
        {
            AbilityEffectType.Damage => ApplyDamage(world),
            AbilityEffectType.Healing => ApplyHealing(world),
            _ => [world.CreateIdleLogEntry(actor, IdleReasons.InvalidAction)]
        };
    }

    private List<GameActionLogEntry> ApplyDamage(ArenaWorld world)
    {
        decimal damage = ability.Get(AbilityStatType.Damage);

        StatSnapshot targetHp = target.Stats.ApplyDamage(damage);

        var entries = new List<GameActionLogEntry>
        {
            world.CreateAbilityUsedLogEntry(
                actor,
                target,
                ability,
                damage,
                targetHp)
        };

        if (target.IsDead)
        {
            target.MarkKilledBy(actor.Id);
            entries.Add(world.CreateDeathLogEntry(target));
        }

        return entries;
    }

    private IReadOnlyList<GameActionLogEntry> ApplyHealing(ArenaWorld world)
    {
        decimal healing = ability.Get(AbilityStatType.Healing);

        StatSnapshot targetHp = target.Stats.Heal(healing);

        return
        [
            world.CreateAbilityUsedLogEntry(
                actor,
                target,
                ability,
                healing,
                targetHp)
        ];
    }

    private static FacingDirection GetFacingDirection(Position from, Position to)
    {
        if (to.X > from.X)
        {
            return FacingDirection.Right;
        }

        if (to.X < from.X)
        {
            return FacingDirection.Left;
        }

        return FacingDirection.Right;
    }
}
