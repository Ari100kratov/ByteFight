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
    public void CalculateFacing_ShouldSnapToEightDirections()
    {
        var origin = new Position(2, 2);

        origin.CalculateFacing(new Position(1, 2)).ShouldBe(FacingDirection.DownLeft);
        origin.CalculateFacing(new Position(3, 2)).ShouldBe(FacingDirection.DownRight);
        origin.CalculateFacing(new Position(2, 1)).ShouldBe(FacingDirection.Up);
        origin.CalculateFacing(new Position(2, 3)).ShouldBe(FacingDirection.Down);
    }

    [Fact]
    public void HexDistance_ShouldMatchHexGeometry()
    {
        var origin = new Position(1, 1);
        var target = new Position(4, 5);

        origin.HexDistance(target).ShouldBe(HexGeometry.Distance(origin, target));
    }

    [Fact]
    public void GetHexNeighbors_ShouldReturnSixNeighbors()
    {
        var neighbors = new Position(2, 2)
            .GetHexNeighbors()
            .ToList();

        neighbors.Count.ShouldBe(6);
    }

    [Fact]
    public void GetNeighbors4_ShouldNotReturnNegativeCoordinates()
    {
        var neighbors = new Position(0, 0)
            .GetNeighbors4()
            .ToList();

        neighbors.ShouldBe([
            new Position(1, 0),
            new Position(0, 1)
        ]);
    }
}
