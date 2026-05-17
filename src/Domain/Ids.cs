namespace Domain;

/// <summary>
/// Строго типизированный идентификатор пользователя.
/// </summary>
public readonly record struct UserId(Guid Value);

/// <summary>
/// Строго типизированный идентификатор арены.
/// </summary>
public readonly record struct ArenaId(Guid Value);

/// <summary>
/// Строго типизированный идентификатор юнита в игровой сессии.
/// </summary>
public readonly record struct UnitId(Guid Value);
