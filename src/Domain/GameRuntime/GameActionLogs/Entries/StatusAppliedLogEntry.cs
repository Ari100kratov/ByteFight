using Domain.Game.Statuses;
using Domain.ValueObjects;

namespace Domain.GameRuntime.GameActionLogs.Entries;

/// <summary>
/// Запись журнала о наложении статус-эффекта на юнита.
/// </summary>
public sealed class StatusAppliedLogEntry : GameActionLogEntry
{
    public UnitId TargetId { get; private set; }

    public string TargetName { get; private set; }

    public StatusEffectType StatusType { get; private set; }

    /// <summary>
    /// Длительность эффекта в ходах.
    /// </summary>
    public int Duration { get; private set; }

    /// <summary>
    /// Сила эффекта.
    /// </summary>
    public decimal Magnitude { get; private set; }

    /// <summary>
    /// Здоровье цели после применения эффекта (для моментальных эффектов).
    /// </summary>
    public StatSnapshot? TargetHp { get; private set; }

    private StatusAppliedLogEntry() { } // EF

    public StatusAppliedLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        UnitId targetId,
        string targetName,
        StatusEffectType statusType,
        int duration,
        decimal magnitude,
        StatSnapshot? targetHp,
        int turnIndex)
        : base(
            sessionId,
            actorId,
            actorName,
            GameActionLogEntryType.StatusApplied,
            info,
            turnIndex)
    {
        TargetId = targetId;
        TargetName = targetName;
        StatusType = statusType;
        Duration = duration;
        Magnitude = magnitude;
        TargetHp = targetHp;
    }
}
