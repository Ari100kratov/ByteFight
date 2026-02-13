using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Базовый тип действия, которое пользовательский скрипт возвращает на текущем ходу.
/// </summary>
public abstract record UserAction;

/// <summary>
/// Действие атаки цели по её идентификатору.
/// </summary>
/// <param name="TargetId">Идентификатор цели для атаки.</param>
public sealed record Attack(Guid TargetId) : UserAction;

/// <summary>
/// Действие перемещения к указанной клетке.
/// </summary>
/// <param name="Target">Целевая позиция перемещения.</param>
public sealed record MoveTo(Position Target) : UserAction;

/// <summary>
/// Действие бездействия — пропуск текущего хода.
/// </summary>
public sealed record Idle() : UserAction;
