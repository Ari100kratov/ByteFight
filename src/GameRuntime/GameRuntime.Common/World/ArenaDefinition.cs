using Domain;
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

    private readonly List<ArenaItemDefinition> items = [];
    public required IReadOnlyList<ArenaItemDefinition> Items
    {
        get => items;
        init => items = [.. value];
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
