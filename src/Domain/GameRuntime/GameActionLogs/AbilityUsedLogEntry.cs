using Domain.Game.Abilities;
using Domain.ValueObjects;

namespace Domain.GameRuntime.GameActionLogs;

/// <summary>
/// Запись журнала о применении способности.
/// </summary>
public sealed class AbilityUsedLogEntry : GameActionLogEntry
{
    public AbilityType AbilityType { get; private set; }
    public AbilityEffectType EffectType { get; private set; }
    public string? AbilityName { get; private set; }

    public UnitId TargetId { get; private set; }
    public string TargetName { get; private set; }

    /// <summary>
    /// Значение эффекта: урон, лечение, щит и т.д.
    /// </summary>
    public decimal Value { get; private set; }

    public FacingDirection FacingDirection { get; private set; }
    public StatSnapshot TargetHp { get; private set; }

    private AbilityUsedLogEntry() { } // EF

    public AbilityUsedLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        AbilityType abilityType,
        AbilityEffectType effectType,
        string? abilityName,
        UnitId targetId,
        string targetName,
        decimal value,
        FacingDirection facingDirection,
        StatSnapshot targetHp,
        int turnIndex)
        : base(
            sessionId,
            actorId,
            actorName,
            GameActionLogEntryType.AbilityUsed,
            info,
            turnIndex)
    {
        AbilityType = abilityType;
        EffectType = effectType;
        AbilityName = abilityName;
        TargetId = targetId;
        TargetName = targetName;
        Value = value;
        FacingDirection = facingDirection;
        TargetHp = targetHp;
    }
}
