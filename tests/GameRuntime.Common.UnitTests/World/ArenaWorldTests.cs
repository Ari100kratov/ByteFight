using Domain.GameRuntime.GameResults;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;
using SharedKernel;
using Shouldly;
using Xunit;

namespace GameRuntime.Common.UnitTests.World;

public sealed class ArenaWorldTests
{
    [Fact]
    public void GetUnit_ShouldReturnPlayerOrEnemyByRuntimeId()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy]);

        world.GetUnit(player.Id).ShouldBe(player);
        world.GetUnit(enemy.Id).ShouldBe(enemy);
        Should.Throw<DomainException>(() => world.GetUnit(Guid.CreateVersion7())).Code.ShouldBe("UNIT_NOT_FOUND");
    }

    [Fact]
    public void CheckGameOver_ShouldReturnVictoryWhenAllEnemiesAreDead()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy]);

        enemy.Stats.ApplyDamage(100);

        GameResult? result = world.CheckGameOver();

        result.ShouldNotBeNull();
        result.Outcome.ShouldBe(GameOutcome.Victory);
        result.WinnerUnitId?.Value.ShouldBe(player.Id);
    }

    [Fact]
    public void CheckGameOver_ShouldReturnDefeatWithKillerWhenPlayerIsDead()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy]);

        player.Stats.ApplyDamage(100);
        player.MarkKilledBy(enemy.Id);

        GameResult? result = world.CheckGameOver();

        result.ShouldNotBeNull();
        result.Outcome.ShouldBe(GameOutcome.Defeat);
        result.WinnerUnitId?.Value.ShouldBe(enemy.Id);
    }

    [Fact]
    public void CheckGameOver_ShouldRejectDeadPlayerWithoutKiller()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy]);

        player.Stats.ApplyDamage(100);

        Should.Throw<DomainException>(() => world.CheckGameOver()).Code.ShouldBe("GAME_RESULT_INVALID_KILLER");
    }

    [Fact]
    public void CheckGameOver_ShouldReturnTurnLimitLossWhenLimitIsReached()
    {
        ArenaWorld world = TestWorldFactory.CreateWorld(
            TestWorldFactory.CreatePlayer(new Position(0, 0)),
            [TestWorldFactory.CreateEnemy(new Position(1, 0))]);

        for (int i = 0; i < world.Arena.MaxTurnsCount; i++)
        {
            world.IncrementTurn();
        }

        world.CheckGameOver()?.Outcome.ShouldBe(GameOutcome.TurnLimitLoss);
    }

    [Fact]
    public void GetReachableCells_ShouldExcludeBlockedAndOccupiedCells()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), moveRange: 2);
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 1));
        ArenaWorld world = TestWorldFactory.CreateWorld(
            player,
            [enemy],
            gridWidth: 4,
            gridHeight: 4,
            blockedPositions: [new Position(0, 1)]);

        IReadOnlySet<Position> cells = world.GetReachableCells(player, player.Position, maxDistance: 2);

        cells.ShouldContain(new Position(0, 0));
        cells.ShouldContain(new Position(1, 0));
        cells.ShouldContain(new Position(2, 0));
        cells.ShouldNotContain(new Position(0, 1));
        cells.ShouldNotContain(new Position(1, 1));
    }
}
