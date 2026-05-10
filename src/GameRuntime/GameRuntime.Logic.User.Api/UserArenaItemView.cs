using Domain.Game.ArenaItems;
using Domain.ValueObjects;
using SharedKernel;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Предмет на арене
/// </summary>
[UserCodeApi]
public sealed record UserArenaItemView
{
    /// <summary>
    /// Уникальный идентификатор предмета.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Наименование
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Тип предмета.
    /// </summary>
    public required ArenaItemType Type { get; init; }

    /// <summary>
    /// Позиция предмета на арене.
    /// </summary>
    public required Position Position { get; init; }

    /// <summary>
    /// Основное значение эффекта предмета.
    /// </summary>
    public required int Value { get; init; }
}
