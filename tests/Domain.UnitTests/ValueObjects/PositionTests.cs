using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace Domain.UnitTests.ValueObjects;

public sealed class PositionTests
{
    [Fact]
    public void Constructor_ShouldRejectNegativeCoordinates()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Position(-1, 0));
        Should.Throw<ArgumentOutOfRangeException>(() => new Position(0, -1));
    }

    [Fact]
    public void DistanceMethods_ShouldCalculateGridDistances()
    {
        var origin = new Position(1, 1);
        var target = new Position(4, 5);

        origin.ManhattanDistance(target).ShouldBe(7);
        origin.EuclideanDistance(target).ShouldBe(5, tolerance: 0.0001);
    }

    [Fact]
    public void CalculateFacing_ShouldUseHorizontalDirection()
    {
        var origin = new Position(2, 2);

        origin.CalculateFacing(new Position(1, 2)).ShouldBe(FacingDirection.Left);
        origin.CalculateFacing(new Position(3, 2)).ShouldBe(FacingDirection.Right);
        origin.CalculateFacing(new Position(2, 1)).ShouldBe(FacingDirection.Right);
    }

    [Fact]
    public void GetNeighbors4_ShouldNotReturnNegativeCoordinates()
    {
        IReadOnlyList<Position> neighbors = new Position(0, 0)
            .GetNeighbors4()
            .ToList();

        neighbors.ShouldBe([
            new Position(1, 0),
            new Position(0, 1)
        ]);
    }
}
