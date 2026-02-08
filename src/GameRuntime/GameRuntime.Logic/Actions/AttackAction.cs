using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.Core.Units;

namespace GameRuntime.Logic.Actions;

public sealed class AttackAction : IRuntimeAction
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

    public IEnumerable<RuntimeLogEntry> Execute()
    {
        FacingDirection facing = Actor.Position.CalculateFacing(TargetUnit.Position);
        Actor.Turn(facing);

        StatSnapshot hpSnapshot = TargetUnit.Stats.ApplyDamage(Damage);

        yield return new AttackLogEntry(
            ActorId: Actor.Id,
            TargetId: TargetUnit.Id,
            Damage: Damage,
            FacingDirection: facing,
            TargetHp: hpSnapshot
        );

        if (TargetUnit.IsDead)
        {
            TargetUnit.MarkKilledBy(Actor.Id);
            yield return new DeathLogEntry(ActorId: TargetUnit.Id);
        }
    }
}
