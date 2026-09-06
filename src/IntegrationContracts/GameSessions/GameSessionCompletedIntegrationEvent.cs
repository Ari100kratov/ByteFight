using System.Text.Json.Serialization;

namespace IntegrationContracts.GameSessions;

/// <summary>
/// Стабильный интеграционный контракт о завершении игровой сессии.
/// </summary>
public sealed record GameSessionCompletedIntegrationEvent
{
    /// <summary>
    /// Версия схемы интеграционного события.
    /// </summary>
    public required int SchemaVersion { get; init; }

    /// <summary>
    /// Идентификатор события.
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// Идентификатор игровой сессии.
    /// </summary>
    public required Guid SessionId { get; init; }

    /// <summary>
    /// Время создания интеграционного события.
    /// </summary>
    public required DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Идентификатор арены.
    /// </summary>
    public required Guid ArenaId { get; init; }

    /// <summary>
    /// Время начала сессии.
    /// </summary>
    public required DateTime StartedAtUtc { get; init; }

    /// <summary>
    /// Время завершения сессии.
    /// </summary>
    public required DateTime EndedAtUtc { get; init; }

    /// <summary>
    /// Общее число ходов.
    /// </summary>
    public required int TotalTurns { get; init; }

    /// <summary>
    /// Итог сессии.
    /// </summary>
    public required IntegrationGameOutcome Outcome { get; init; }

    /// <summary>
    /// Идентификатор победившего юнита.
    /// </summary>
    public Guid? WinnerUnitId { get; init; }

    /// <summary>
    /// Участники игровой сессии.
    /// </summary>
    public required IReadOnlyList<GameSessionParticipantSnapshot> Participants { get; init; }

    /// <summary>
    /// Снимок журнала боя.
    /// </summary>
    public required IReadOnlyList<GameActionLogEntrySnapshot> Logs { get; init; }
}

/// <summary>
/// Участник завершённой игровой сессии.
/// </summary>
public sealed record GameSessionParticipantSnapshot(
    Guid UnitId,
    Guid? UserId,
    IntegrationParticipantUnitType UnitType,
    string DisplayName,
    string? UserFirstName = null,
    string? UserLastName = null,
    string? CharacterClassName = null,
    string? CharacterSpecName = null);

/// <summary>
/// Тип участника в интеграционном контракте.
/// </summary>
public enum IntegrationParticipantUnitType
{
    Player = 1,
    Npc = 2
}

/// <summary>
/// Исход игровой сессии в интеграционном контракте.
/// </summary>
public enum IntegrationGameOutcome
{
    Victory = 1,
    Defeat = 2,
    Draw = 3,
    TimeoutLoss = 4,
    TurnLimitLoss = 5
}

/// <summary>
/// Базовый snapshot записи журнала боя.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(IdleLogEntrySnapshot), "idle")]
[JsonDerivedType(typeof(WalkLogEntrySnapshot), "walk")]
[JsonDerivedType(typeof(DeathLogEntrySnapshot), "death")]
[JsonDerivedType(typeof(AbilityUsedLogEntrySnapshot), "ability_used")]
[JsonDerivedType(typeof(ItemPickedUpLogEntrySnapshot), "item_picked_up")]
[JsonDerivedType(typeof(StatusAppliedLogEntrySnapshot), "status_applied")]
[JsonDerivedType(typeof(RoundStartedLogEntrySnapshot), "round_started")]
public abstract record GameActionLogEntrySnapshot(
    Guid Id,
    Guid SessionId,
    Guid ActorId,
    string ActorName,
    string EntryType,
    string? Info,
    int TurnIndex,
    DateTime CreatedAtUtc);

/// <summary>
/// Snapshot пропуска хода.
/// </summary>
public sealed record IdleLogEntrySnapshot(
    Guid Id,
    Guid SessionId,
    Guid ActorId,
    string ActorName,
    string EntryType,
    string? Info,
    int TurnIndex,
    DateTime CreatedAtUtc)
    : GameActionLogEntrySnapshot(Id, SessionId, ActorId, ActorName, EntryType, Info, TurnIndex, CreatedAtUtc);

/// <summary>
/// Snapshot смерти юнита.
/// </summary>
public sealed record DeathLogEntrySnapshot(
    Guid Id,
    Guid SessionId,
    Guid ActorId,
    string ActorName,
    string EntryType,
    string? Info,
    int TurnIndex,
    DateTime CreatedAtUtc)
    : GameActionLogEntrySnapshot(Id, SessionId, ActorId, ActorName, EntryType, Info, TurnIndex, CreatedAtUtc);

/// <summary>
/// Snapshot перемещения.
/// </summary>
public sealed record WalkLogEntrySnapshot(
    Guid Id,
    Guid SessionId,
    Guid ActorId,
    string ActorName,
    string EntryType,
    string? Info,
    int TurnIndex,
    DateTime CreatedAtUtc,
    string FacingDirection,
    PositionSnapshot To)
    : GameActionLogEntrySnapshot(Id, SessionId, ActorId, ActorName, EntryType, Info, TurnIndex, CreatedAtUtc);

/// <summary>
/// Snapshot применения способности.
/// </summary>
public sealed record AbilityUsedLogEntrySnapshot(
    Guid Id,
    Guid SessionId,
    Guid ActorId,
    string ActorName,
    string EntryType,
    string? Info,
    int TurnIndex,
    DateTime CreatedAtUtc,
    string AbilityType,
    string EffectType,
    string? AbilityName,
    Guid TargetId,
    string TargetName,
    decimal Value,
    string FacingDirection,
    StatSnapshotContract TargetHp)
    : GameActionLogEntrySnapshot(Id, SessionId, ActorId, ActorName, EntryType, Info, TurnIndex, CreatedAtUtc);

/// <summary>
/// Snapshot подбора предмета.
/// </summary>
public sealed record ItemPickedUpLogEntrySnapshot(
    Guid Id,
    Guid SessionId,
    Guid ActorId,
    string ActorName,
    string EntryType,
    string? Info,
    int TurnIndex,
    DateTime CreatedAtUtc,
    Guid PlacedItemId,
    Guid ItemId,
    string ItemName,
    string ItemType,
    PositionSnapshot Position,
    decimal Value,
    StatSnapshotContract ActorHp)
    : GameActionLogEntrySnapshot(Id, SessionId, ActorId, ActorName, EntryType, Info, TurnIndex, CreatedAtUtc);

/// <summary>
/// Snapshot наложения статус-эффекта.
/// </summary>
public sealed record StatusAppliedLogEntrySnapshot(
    Guid Id,
    Guid SessionId,
    Guid ActorId,
    string ActorName,
    string EntryType,
    string? Info,
    int TurnIndex,
    DateTime CreatedAtUtc,
    Guid TargetId,
    string TargetName,
    string StatusType,
    int Duration,
    decimal Magnitude)
    : GameActionLogEntrySnapshot(Id, SessionId, ActorId, ActorName, EntryType, Info, TurnIndex, CreatedAtUtc);

/// <summary>
/// Snapshot начала раунда.
/// </summary>
public sealed record RoundStartedLogEntrySnapshot(
    Guid Id,
    Guid SessionId,
    Guid ActorId,
    string ActorName,
    string EntryType,
    string? Info,
    int TurnIndex,
    DateTime CreatedAtUtc,
    int RoundNumber)
    : GameActionLogEntrySnapshot(Id, SessionId, ActorId, ActorName, EntryType, Info, TurnIndex, CreatedAtUtc);

/// <summary>
/// Снимок позиции.
/// </summary>
public sealed record PositionSnapshot(int X, int Y);

/// <summary>
/// Снимок характеристики.
/// </summary>
public sealed record StatSnapshotContract(decimal Current, decimal Max);
