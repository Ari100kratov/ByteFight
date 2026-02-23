using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.World;
using GameRuntime.World.Units;

namespace GameRuntime.Logic.Actions;

internal sealed class AttackAction : IRuntimeAction
{
    public BaseUnit Actor { get; }
    public BaseUnit TargetUnit { get; }
    public decimal Damage { get; }

    public AttackAction(BaseUnit actor, BaseUnit targetUnit, decimal damage)
    {
        Actor = actor;
        TargetUnit = targetUnit;
        Damage = damage;
    }

    public IEnumerable<GameActionLogEntry> Execute(ArenaWorld world)
    {
        FacingDirection facing = Actor.Position.CalculateFacing(TargetUnit.Position);
        Actor.Turn(facing);

        StatSnapshot hpSnapshot = TargetUnit.Stats.ApplyDamage(Damage);

        yield return world.CreateAttackLogEntry(
            actorId: Actor.Id,
            targetId: TargetUnit.Id,
            damage: Damage,
            facingDirection: facing,
            targetHp: hpSnapshot
        );

        if (TargetUnit.IsDead)
        {
            TargetUnit.MarkKilledBy(Actor.Id);
            yield return world.CreateDeathLogEntry(TargetUnit.Id);
        }
    }
}
