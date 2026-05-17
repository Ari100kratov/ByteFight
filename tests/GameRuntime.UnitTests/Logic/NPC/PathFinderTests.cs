using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.NPC.PathFinding;
using Shouldly;
using Xunit;

namespace GameRuntime.UnitTests.Logic.NPC;

public sealed class PathFinderTests
{
    [Fact]
    public void FindPath_ShouldAvoidBlockedAndOccupiedCells()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 1));
        ArenaWorld world = TestWorldFactory.CreateWorld(
            player,
            [enemy],
            gridWidth: 4,
            gridHeight: 3,
            blockedPositions: [new Position(1, 0)]);

        List<Position>? path = new PathFinder().FindPath(world, player.Position, new Position(3, 0));

        path.ShouldNotBeNull();
        path.ShouldNotContain(new Position(1, 0));
        path.ShouldNotContain(enemy.Position);
        path[0].ShouldBe(player.Position);
        path[^1].ShouldBe(new Position(3, 0));
    }

    [Fact]
    public void FindPathTowards_ShouldStopAtReachableCellWhenTargetIsOccupied()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(4, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy], gridWidth: 5, gridHeight: 2);

        List<Position>? path = new PathFinder().FindPathTowards(world, enemy.Position, player.Position);

        path.ShouldNotBeNull();
        path[0].ShouldBe(enemy.Position);
        path[^1].ShouldNotBe(player.Position);
        path[^1].ManhattanDistance(player.Position).ShouldBeLessThan(enemy.Position.ManhattanDistance(player.Position));
    }
}
