namespace Chronicles.Application.Abstractions.Data;

/// <summary>
/// Нормализованная запись журнала завершённой игровой сессии для расчёта номинаций.
/// </summary>
public sealed record CompletedGameSessionLogEntryData(
    Guid ActorId,
    CompletedGameSessionLogEntryKind Kind,
    string EntryType,
    int TurnIndex,
    Guid? TargetId,
    decimal Value,
    string? EffectType,
    string? ItemType);
