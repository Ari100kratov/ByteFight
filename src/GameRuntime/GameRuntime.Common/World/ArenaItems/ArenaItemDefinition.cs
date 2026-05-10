using Domain.Game.ArenaItems;
using Domain.ValueObjects;

namespace GameRuntime.Common.World.ArenaItems;

/// <summary>
/// Предмет, размещённый на арене во время боя.
/// </summary>
public sealed record ArenaItemDefinition
{
    /// <summary>
    /// Идентификатор размещения предмета на арене
    /// </summary>
    public required Guid PlacedItemId { get; init; }

    /// <summary>
    /// Уникальный идентификатор предмета.
    /// </summary>
    public required Guid ItemId { get; init; }

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
    /// Например, количество лечения для зелья.
    /// </summary>
    public required int Value { get; init; }
}
