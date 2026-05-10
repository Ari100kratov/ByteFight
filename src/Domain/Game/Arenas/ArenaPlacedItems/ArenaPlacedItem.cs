using Domain.Game.ArenaItems;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Game.Arenas.ArenaPlacedItems;

/// <summary>
/// Размещение предмета на конкретной арене.
/// </summary>
public sealed class ArenaPlacedItem : Entity
{
    /// <summary>
    /// Уникальный идентификатор размещения.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор арены.
    /// </summary>
    public Guid ArenaId { get; set; }

    /// <summary>
    /// Арена.
    /// </summary>
    public Arena Arena { get; set; } = null!;

    /// <summary>
    /// Идентификатор предмета.
    /// </summary>
    public Guid ItemId { get; set; }

    /// <summary>
    /// Предмет.
    /// </summary>
    public ArenaItem Item { get; set; } = null!;

    /// <summary>
    /// Позиция предмета на арене.
    /// </summary>
    public Position Position { get; set; }
}
