using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;
using Shouldly;
using Xunit;

namespace GameRuntime.Common.UnitTests;

public sealed class MovementRulesTests
{
    [Fact]
    public void CanStandOn_ShouldRejectOutsideBlockedAndOccupiedCells()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy], blockedPositions: [new Position(2, 0)]);

        MovementRules.CanStandOn(world, player, new Position(3, 0)).ShouldBeFalse();
        MovementRules.CanStandOn(world, player, new Position(2, 0)).ShouldBeFalse();
        MovementRules.CanStandOn(world, player, new Position(1, 0)).ShouldBeFalse();

        enemy.Stats.ApplyDamage(100);

        MovementRules.CanStandOn(world, player, new Position(1, 0)).ShouldBeTrue();
    }

    [Fact]
    public void SelectMoveTarget_ShouldStopBeforeFirstInvalidCell()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [], gridWidth: 5, gridHeight: 1, blockedPositions: [new Position(2, 0)]);
        Position[] path =
        [
            new(0, 0),
            new(1, 0),
            new(2, 0),
            new(3, 0)
        ];

        Position? target = MovementRules.SelectMoveTarget(world, player, path, moveRange: 3);

        target.ShouldBe(new Position(1, 0));
    }

    [Fact]
    public void SelectMoveTarget_ShouldRespectMoveRange()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [], gridWidth: 5, gridHeight: 1);
        Position[] path =
        [
            new(0, 0),
            new(1, 0),
            new(2, 0),
            new(3, 0)
        ];

        Position? target = MovementRules.SelectMoveTarget(world, player, path, moveRange: 2);

        target.ShouldBe(new Position(2, 0));
    }
}
