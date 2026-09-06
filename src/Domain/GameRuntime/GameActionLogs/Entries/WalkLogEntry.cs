using Domain.ValueObjects;

namespace Domain.GameRuntime.GameActionLogs.Entries;

/// <summary>
/// Запись журнала о перемещении юнита.
/// </summary>
public sealed class WalkLogEntry : GameActionLogEntry
{
    public FacingDirection FacingDirection { get; private set; }

    public Position To { get; private set; }

    /// <summary>
    /// Полный путь перемещения по гексам (включая стартовую клетку).
    /// Клиент использует его для плавной анимации.
    /// </summary>
    public IReadOnlyList<Position>? Path { get; private set; }

    private WalkLogEntry() { } // EF

    public WalkLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        FacingDirection facingDirection,
        Position to,
        int turnIndex,
        IReadOnlyList<Position>? path = null)
        : base(
            sessionId,
            actorId,
            actorName,
            GameActionLogEntryType.Walk,
            info,
            turnIndex)
    {
        FacingDirection = facingDirection;
        To = to;
        Path = path;
    }
}
