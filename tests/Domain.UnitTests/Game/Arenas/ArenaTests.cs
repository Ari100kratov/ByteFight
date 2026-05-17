using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaPlacedItems;
using Domain.Game.GameModes;
using Domain.ValueObjects;
using SharedKernel;
using Shouldly;
using Xunit;

namespace Domain.UnitTests.Game.Arenas;

public sealed class ArenaTests
{
    [Fact]
    public void SetSize_ShouldRejectNonPositiveDimensions()
    {
        Arena arena = CreateArena();

        Should.Throw<DomainException>(() => arena.SetSize(0, 5)).Code.ShouldBe("invalid_grid_width");
        Should.Throw<DomainException>(() => arena.SetSize(5, 0)).Code.ShouldBe("invalid_grid_height");
    }

    [Fact]
    public void SetSize_ShouldValidateExistingPositionsBeforeChangingGrid()
    {
        Arena arena = CreateArena();
        arena.SetSize(5, 5);
        arena.SetStartPosition(new Position(4, 4));

        DomainException exception = Should.Throw<DomainException>(() => arena.SetSize(4, 5));

        exception.Code.ShouldBe("start_position_out_of_bounds");
        arena.GridWidth.ShouldBe(5);
        arena.GridHeight.ShouldBe(5);
    }

    [Fact]
    public void SetBlockedPositions_ShouldRejectOutOfBoundsCells()
    {
        Arena arena = CreateArena();
        arena.SetSize(3, 3);

        DomainException exception = Should.Throw<DomainException>(
            () => arena.SetBlockedPositions([new Position(2, 2), new Position(3, 1)]));

        exception.Code.ShouldBe("blocked_position_out_of_bounds");
    }

    [Fact]
    public void SetSize_ShouldValidatePlacedItems()
    {
        Arena arena = CreateArena();
        arena.SetSize(5, 5);
        arena.Items.Add(new ArenaPlacedItem
        {
            Id = Guid.CreateVersion7(),
            ArenaId = arena.Id,
            Position = new Position(4, 4)
        });

        DomainException exception = Should.Throw<DomainException>(() => arena.SetSize(4, 4));

        exception.Code.ShouldBe("arena_item_position_out_of_bounds");
    }

    private static Arena CreateArena() =>
        new()
        {
            Id = Guid.CreateVersion7(),
            Name = "Training",
            BackgroundAsset = "arena.png",
            ImageUrl = "https://example.test/arena.png",
            GameModes = [GameModeType.Training],
            CreatedBy = new UserId(Guid.CreateVersion7()),
            CreatedAt = new DateTime(2026, 5, 16, 10, 0, 0, DateTimeKind.Utc),
            Enemies = []
        };
}
