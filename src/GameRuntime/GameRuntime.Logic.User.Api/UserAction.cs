using System.Text.Json.Serialization;
using Domain.ValueObjects;
using SharedKernel;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Базовый тип действия юнита.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Attack), "attack")]
[JsonDerivedType(typeof(MoveTo), "moveTo")]
[JsonDerivedType(typeof(MoveTowards), "moveTowards")]
[JsonDerivedType(typeof(MoveTowardsPosition), "moveTowardsPosition")]
[JsonDerivedType(typeof(MoveAwayFrom), "moveAwayFrom")]
[JsonDerivedType(typeof(Idle), "idle")]
public abstract record UserAction;

/// <summary>
/// Атака юнита по её идентификатору.
/// </summary>
/// <param name="TargetId">Идентификатор юнита для атаки.</param>
[UserCodeApi]
public sealed record Attack(Guid TargetId) : UserAction;

/// <summary>
/// Перемещение к указанной позиции.
///
/// Это строгое перемещение:
/// если путь до клетки отсутствует, действие завершается без движения.
/// </summary>
/// <param name="Target">Целевая позиция перемещения.</param>
[UserCodeApi]
public sealed record MoveTo(Position Target) : UserAction;

/// <summary>
/// Переместиться в сторону указанного юнита.
///
/// Это высокоуровневое действие для типового сценария сближения.
/// Пользователь указывает только цель, а движок сам строит путь и выбирает
/// достижимую за ход клетку.
///
/// Если клетка цели занята или рядом с целью нет свободных клеток,
/// действие всё равно пытается приблизиться настолько, насколько это возможно.
/// </summary>
/// <param name="TargetId">Идентификатор юнита, к которому нужно приблизиться.</param>
[UserCodeApi]
public sealed record MoveTowards(Guid TargetId) : UserAction;

/// <summary>
/// Переместиться в сторону указанной позиции.
///
/// В отличие от <see cref="MoveTo"/>, действие не требует обязательно
/// достичь самой клетки. Если путь до позиции невозможен,
/// движок попытается приблизиться к ней настолько, насколько это возможно.
///
/// Удобно для пользовательского AI, когда нужно двигаться в область карты,
/// а не в конкретную достижимую клетку.
/// </summary>
/// <param name="Target">Позиция, в сторону которой нужно двигаться.</param>
[UserCodeApi]
public sealed record MoveTowardsPosition(Position Target) : UserAction;

/// <summary>
/// Переместиться от указанного юнита.
///
/// Это высокоуровневое действие для сценария отступления:
/// пользователь указывает опасную цель, а движок сам выбирает
/// достижимую за ход клетку, которая увеличивает дистанцию до неё.
/// </summary>
/// <param name="TargetId">Идентификатор юнита, от которого нужно отойти.</param>
[UserCodeApi]
public sealed record MoveAwayFrom(Guid TargetId) : UserAction;

/// <summary>
/// Бездействие — пропуск текущего хода.
/// </summary>
[UserCodeApi]
public sealed record Idle() : UserAction;
