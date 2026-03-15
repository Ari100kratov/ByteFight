using Domain.Game.Actions;
using Domain.ValueObjects;

namespace Domain.GameRuntime.GameActionLogs;

public sealed class AttackLogEntry : GameActionLogEntry
{
    public UnitId TargetId { get; private set; }
    public string TargetName { get; private set; }
    public decimal Damage { get; private set; }
    public FacingDirection FacingDirection { get; private set; }
    public StatSnapshot TargetHp { get; private set; }

    private AttackLogEntry() { } // EF

    public AttackLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        UnitId targetId,
        string targetName,
        decimal damage,
        FacingDirection facingDirection,
        StatSnapshot targetHp,
        int turnIndex)
        : base(sessionId, actorId, actorName, ActionType.Attack, info, turnIndex)
    {
        TargetId = targetId;
        TargetName = targetName;
        Damage = damage;
        FacingDirection = facingDirection;
        TargetHp = targetHp;
    }
}
