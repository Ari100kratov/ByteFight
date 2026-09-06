using System.Collections.Immutable;
using Domain.Game.Arenas;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;
using Shouldly;
using Xunit;

namespace GameRuntime.Common.UnitTests;

public sealed class MovementRulesTests
{
    [Fact]
    public void CanStandOn_ShouldRejectBlockedAndOccupiedCells()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(
            player,
            [enemy],
            gridWidth: 5,
            gridHeight: 5,
            blockedPositions: [new Position(2, 0)]);

        MovementRules.CanStandOn(world, player, new Position(1, 0)).ShouldBeFalse();
        MovementRules.CanStandOn(world, player, new Position(2, 0)).ShouldBeFalse();
        MovementRules.CanStandOn(world, player, new Position(0, 1)).ShouldBeTrue();
        MovementRules.CanStandOn(world, player, new Position(9, 9)).ShouldBeFalse();
    }

    [Fact]
    public void CanStandOn_ShouldRejectImpassableTerrain()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(
            player,
            [],
            gridWidth: 5,
            gridHeight: 5,
            terrain: new Dictionary<Position, TerrainType>
            {
                [new Position(1, 0)] = TerrainType.Rock,
                [new Position(0, 1)] = TerrainType.Water
            });

        MovementRules.CanStandOn(world, player, new Position(1, 0)).ShouldBeFalse();
        MovementRules.CanStandOn(world, player, new Position(0, 1)).ShouldBeFalse();
    }

    [Fact]
    public void MovementCost_ShouldReflectTerrain()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(
            player,
            [],
            gridWidth: 5,
            gridHeight: 5,
            terrain: new Dictionary<Position, TerrainType>
            {
                [new Position(1, 0)] = TerrainType.Forest,
                [new Position(0, 1)] = TerrainType.Swamp,
                [new Position(2, 0)] = TerrainType.Rock
            });

        MovementRules.MovementCost(world, new Position(1, 0)).ShouldBe(2);
        MovementRules.MovementCost(world, new Position(0, 1)).ShouldBe(2);
        MovementRules.MovementCost(world, new Position(2, 0)).ShouldBe(int.MaxValue);
        MovementRules.MovementCost(world, new Position(3, 3)).ShouldBe(1);
    }

    [Fact]
    public void GetReachableCells_ShouldRespectMovePoints()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), moveRange: 2);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [], gridWidth: 6, gridHeight: 6);

        player.BattleState.MovePointsRemaining = 2;

        ImmutableHashSet<Position> cells = MovementRules.GetReachableCells(world, player);

        cells.ShouldContain(new Position(1, 0));
        cells.ShouldContain(new Position(0, 1));
        cells.ShouldContain(new Position(1, 1));
        cells.ShouldNotContain(new Position(4, 4));
    }

    [Fact]
    public void GetReachableCells_ExpensiveTerrain_ShouldLimitReach()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), moveRange: 2);
        ArenaWorld world = TestWorldFactory.CreateWorld(
            player,
            [],
            gridWidth: 6,
            gridHeight: 6,
            terrain: new Dictionary<Position, TerrainType>
            {
                // Стена чащи вокруг старта: вход стоит 2 очка.
                [new Position(1, 0)] = TerrainType.Forest,
                [new Position(0, 1)] = TerrainType.Forest
            });

        player.BattleState.MovePointsRemaining = 2;

        ImmutableHashSet<Position> cells = MovementRules.GetReachableCells(world, player);

        cells.ShouldContain(new Position(1, 0));
        cells.ShouldNotContain(new Position(2, 0));
    }

    [Fact]
    public void IsPathValid_ShouldAcceptPathWithinBudget()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), moveRange: 3);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [], gridWidth: 6, gridHeight: 6);

        player.BattleState.MovePointsRemaining = 3;

        List<Position> path = [new(0, 0), new(1, 0), new(2, 0)];

        MovementRules.IsPathValid(world, player, path, out int cost).ShouldBeTrue();
        cost.ShouldBe(2);
    }

    [Fact]
    public void IsPathValid_ShouldRejectPathOverBudget()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), moveRange: 1);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [], gridWidth: 6, gridHeight: 6);

        player.BattleState.MovePointsRemaining = 1;

        List<Position> path = [new(0, 0), new(1, 0), new(2, 0)];

        MovementRules.IsPathValid(world, player, path, out _).ShouldBeFalse();
    }

    [Fact]
    public void IsPathValid_ShouldRejectDisconnectedPath()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [], gridWidth: 6, gridHeight: 6);

        player.BattleState.MovePointsRemaining = 10;

        List<Position> path = [new(0, 0), new(3, 3)];

        MovementRules.IsPathValid(world, player, path, out _).ShouldBeFalse();
    }

    [Fact]
    public void IsPathValid_ShouldRejectPathThroughOccupiedCell()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy], gridWidth: 6, gridHeight: 6);

        player.BattleState.MovePointsRemaining = 10;

        List<Position> path = [new(0, 0), new(1, 0), new(2, 0)];

        MovementRules.IsPathValid(world, player, path, out _).ShouldBeFalse();
    }
}
