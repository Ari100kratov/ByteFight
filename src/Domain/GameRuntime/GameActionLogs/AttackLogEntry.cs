using Domain.Game.Actions;
using Domain.ValueObjects;

namespace Domain.GameRuntime.GameActionLogs;

public sealed class AttackLogEntry : GameActionLogEntry
{
    public Guid TargetId { get; private set; }
    public decimal Damage { get; private set; }
    public FacingDirection FacingDirection { get; private set; }
    public StatSnapshot TargetHp { get; private set; }

    private AttackLogEntry() { } // EF

    public AttackLogEntry(
        Guid sessionId,
        Guid actorId,
        string? info,
        Guid targetId,
        decimal damage,
        FacingDirection facingDirection,
        StatSnapshot targetHp,
        int turnIndex)
        : base(sessionId, actorId, ActionType.Attack, info, turnIndex)
    {
        TargetId = targetId;
        Damage = damage;
        FacingDirection = facingDirection;
        TargetHp = targetHp;
    }
}
