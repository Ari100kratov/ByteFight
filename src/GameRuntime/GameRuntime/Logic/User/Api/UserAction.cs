using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Базовый тип действия юнита.
/// </summary>
public abstract record UserAction;

/// <summary>
/// Атака юнита по её идентификатору.
/// </summary>
/// <param name="TargetId">Идентификатор юнита для атаки.</param>
public sealed record Attack(Guid TargetId) : UserAction;

/// <summary>
/// Перемещение к указанной позиции.
/// </summary>
/// <param name="Target">Целевая позиция перемещения.</param>
public sealed record MoveTo(Position Target) : UserAction;

/// <summary>
/// Бездействие — пропуск текущего хода.
/// </summary>
public sealed record Idle() : UserAction;
