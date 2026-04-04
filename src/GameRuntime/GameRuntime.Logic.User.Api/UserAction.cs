using System.Text.Json.Serialization;
using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Базовый тип действия юнита.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Attack), "attack")]
[JsonDerivedType(typeof(MoveTo), "moveTo")]
[JsonDerivedType(typeof(MoveTowards), "moveTowards")]
[JsonDerivedType(typeof(MoveAwayFrom), "moveAwayFrom")]
[JsonDerivedType(typeof(Idle), "idle")]
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
/// Переместиться в сторону указанного юнита.
///
/// Это высокоуровневое действие для типового сценария сближения:
/// пользователь указывает только цель, а движок сам:
/// находит путь;
/// определяет достижимую за ход точку на этом пути;
/// выполняет перемещение с учетом препятствий и занятых клеток.
///
/// </summary>
/// <param name="TargetId">Идентификатор юнита, к которому нужно приблизиться.</param>
public sealed record MoveTowards(Guid TargetId) : UserAction;

/// <summary>
/// Переместиться от указанного юнита.
///
/// Это высокоуровневое действие для сценария отступления:
/// пользователь указывает опасную цель, а движок сам выбирает
/// достижимую за ход клетку, которая увеличивает дистанцию до неё.
/// 
/// </summary>
/// <param name="TargetId">Идентификатор юнита, от которого нужно отойти.</param>
public sealed record MoveAwayFrom(Guid TargetId) : UserAction;

/// <summary>
/// Бездействие — пропуск текущего хода.
/// </summary>
public sealed record Idle() : UserAction;
