using Domain.Game.Arenas;
using Domain.ValueObjects;
using GameRuntime.Common.World.ArenaItems;
using SharedKernel;

namespace GameRuntime.Common.World;

public sealed record ArenaDefinition
{
    public int MaxTurnsCount { get; } = 100;


    public required Guid ArenaId { get; init; }

    public required int GridWidth { get; init; }

    public required int GridHeight { get; init; }

    public required Position StartPosition { get; init; }

    public required Position[] BlockedPositions { get; init; }

    /// <summary>
    /// Рельеф арены: гекс → тип рельефа.
    /// Гексы без записи считаются луговиной.
    /// </summary>
    public IReadOnlyDictionary<Position, TerrainType> Terrain { get; init; } =
        new Dictionary<Position, TerrainType>();

    private readonly List<ArenaItemDefinition> items = [];
    public required IReadOnlyList<ArenaItemDefinition> Items
    {
        get => items;
        init => items.AddRange(value);
    }

    /// <summary>
    /// Возвращает тип рельефа гекса.
    /// </summary>
    public TerrainType TerrainAt(Position position) =>
        Terrain.GetValueOrDefault(position, TerrainType.Meadow);

    /// <summary>
    /// Проверяет, находится ли позиция в границах арены.
    /// </summary>
    public bool IsWithin(Position position) =>
        position.IsWithinGrid(GridWidth, GridHeight);

    /// <summary>
    /// Возвращает все гексы арены.
    /// </summary>
    public IEnumerable<Position> AllCells()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                yield return new Position(x, y);
            }
        }
    }

    public ArenaItemDefinition? GetItemAt(Position position)
        => items.FirstOrDefault(x => x.Position == position);

    public ArenaItemDefinition RemoveItem(Guid placedItemId)
    {
        ArenaItemDefinition? item = items.FirstOrDefault(x => x.PlacedItemId == placedItemId)
            ?? throw new DomainException(
                "ARENA_ITEM_NOT_FOUND",
                $"Arena item with id {placedItemId} was not found in this arena.");

        items.Remove(item);

        return item;
    }

    public ArenaItemDefinition RemoveItemAt(Position position)
    {
        ArenaItemDefinition? item = GetItemAt(position)
            ?? throw new DomainException(
                "ARENA_ITEM_NOT_FOUND_AT_POSITION",
                $"Arena item at position ({position.X}, {position.Y}) was not found.");

        items.Remove(item);

        return item;
    }
}
