using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace Domain.UnitTests.ValueObjects;

public sealed class HexGeometryTests
{
    [Fact]
    public void Distance_NeighborCells_ShouldBeOne()
    {
        foreach (Position neighbor in HexGeometry.GetNeighbors(new Position(3, 3)))
        {
            HexGeometry.Distance(new Position(3, 3), neighbor).ShouldBe(1);
        }
    }

    [Theory]
    [InlineData(0, 0, 1, 0, 1)]
    [InlineData(0, 0, 0, 1, 1)]
    [InlineData(2, 2, 1, 3, 2)]
    [InlineData(0, 0, 4, 2, 4)]
    [InlineData(5, 5, 5, 5, 0)]
    public void Distance_ShouldCalculateHexDistance(int x1, int y1, int x2, int y2, int expected)
    {
        HexGeometry.Distance(new Position(x1, y1), new Position(x2, y2))
            .ShouldBe(expected);
    }

    [Fact]
    public void Distance_ShouldBeSymmetric()
    {
        var a = new Position(1, 2);
        var b = new Position(6, 4);

        HexGeometry.Distance(a, b).ShouldBe(HexGeometry.Distance(b, a));
    }

    [Fact]
    public void GetNeighbors_EvenColumn_ShouldReturnSixNeighbors()
    {
        var neighbors = HexGeometry
            .GetNeighbors(new Position(2, 2))
            .ToList();

        neighbors.Count.ShouldBe(6);
        neighbors.ShouldContain(new Position(3, 2));
        neighbors.ShouldContain(new Position(3, 1));
        neighbors.ShouldContain(new Position(2, 1));
        neighbors.ShouldContain(new Position(1, 1));
        neighbors.ShouldContain(new Position(1, 2));
        neighbors.ShouldContain(new Position(2, 3));
    }

    [Fact]
    public void GetNeighbors_OddColumn_ShouldReturnSixNeighbors()
    {
        var neighbors = HexGeometry
            .GetNeighbors(new Position(1, 1))
            .ToList();

        neighbors.Count.ShouldBe(6);
        neighbors.ShouldContain(new Position(2, 2));
        neighbors.ShouldContain(new Position(2, 1));
        neighbors.ShouldContain(new Position(1, 0));
        neighbors.ShouldContain(new Position(0, 1));
        neighbors.ShouldContain(new Position(0, 2));
        neighbors.ShouldContain(new Position(1, 2));
    }

    [Fact]
    public void GetNeighbors_AtOrigin_ShouldSkipNegativeCells()
    {
        var neighbors = HexGeometry
            .GetNeighbors(new Position(0, 0))
            .ToList();

        neighbors.ShouldBe([new Position(1, 0), new Position(0, 1)]);
    }

    [Fact]
    public void GetRing_RadiusOne_ShouldMatchNeighbors()
    {
        var center = new Position(4, 4);

        var ring = HexGeometry.GetRing(center, 1).ToList();
        var neighbors = HexGeometry.GetNeighbors(center).ToList();

        ring.Count.ShouldBe(neighbors.Count);
        ring.OrderBy(p => p.X).ThenBy(p => p.Y)
            .ShouldBe(neighbors.OrderBy(p => p.X).ThenBy(p => p.Y));
    }

    [Fact]
    public void GetRing_RadiusTwo_ShouldContainTwelveCells()
    {
        HexGeometry.GetRing(new Position(3, 3), 2).Count().ShouldBe(12);
    }

    [Fact]
    public void GetCellsInRange_ShouldContainAllCellsUpToRadius()
    {
        var center = new Position(5, 5);

        var cells = HexGeometry.GetCellsInRange(center, 2).ToList();

        cells.Count.ShouldBe(19); // 1 + 6 + 12
        cells.ShouldContain(center);
        cells.Distinct().Count().ShouldBe(19);
    }

    [Fact]
    public void GetLine_Horizontal_ShouldPassThroughColumnCells()
    {
        List<Position> line = HexGeometry.GetLine(new Position(0, 0), new Position(3, 0));

        line.ShouldBe([new Position(0, 0), new Position(1, 0), new Position(2, 0), new Position(3, 0)]);
    }

    [Fact]
    public void GetLine_ShouldStartAndEndAtEndpoints()
    {
        var from = new Position(1, 1);
        var to = new Position(6, 4);

        List<Position> line = HexGeometry.GetLine(from, to);

        line[0].ShouldBe(from);
        line[^1].ShouldBe(to);
        line.Count.ShouldBe(HexGeometry.Distance(from, to) + 1);
    }

    [Fact]
    public void GetLine_SameCell_ShouldReturnSingleCell()
    {
        var cell = new Position(2, 2);

        HexGeometry.GetLine(cell, cell).ShouldBe([cell]);
    }

    [Fact]
    public void GetLine_ConsecutiveCells_ShouldBeNeighbors()
    {
        List<Position> line = HexGeometry.GetLine(new Position(0, 0), new Position(5, 3));

        for (int i = 1; i < line.Count; i++)
        {
            HexGeometry.Distance(line[i - 1], line[i]).ShouldBe(1);
        }
    }

    [Theory]
    [InlineData(0, 0, 1, 0, FacingDirection.DownRight)]
    [InlineData(0, 0, 0, 1, FacingDirection.Down)]
    [InlineData(1, 0, 0, 0, FacingDirection.UpLeft)]
    [InlineData(2, 2, 1, 1, FacingDirection.UpLeft)]
    [InlineData(2, 2, 3, 2, FacingDirection.DownRight)]
    public void GetFacing_ShouldSnapToEightDirections(
        int x1, int y1, int x2, int y2, FacingDirection expected)
    {
        HexGeometry.GetFacing(new Position(x1, y1), new Position(x2, y2))
            .ShouldBe(expected);
    }
}
