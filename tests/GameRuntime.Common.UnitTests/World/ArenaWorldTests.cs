using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameResults;
using Domain.Game.Statuses;
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
    public void StartNextRound_ShouldOrderUnitsByInitiative()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), initiative: 10);
        EnemyUnit fast = TestWorldFactory.CreateEnemy(new Position(1, 0), initiative: 20);
        EnemyUnit slow = TestWorldFactory.CreateEnemy(new Position(2, 0), initiative: 1);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [slow, fast]);

        world.StartNextRound();

        BaseUnit? first = world.AdvanceToNextUnit();
        BaseUnit? second = world.AdvanceToNextUnit();
        BaseUnit? third = world.AdvanceToNextUnit();

        first.ShouldBe(fast);
        second.ShouldBe(player);
        third.ShouldBe(slow);
    }

    [Fact]
    public void StartNextRound_ShouldGivePlayerPriorityOnTie()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), initiative: 7);
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0), initiative: 7);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy]);

        world.StartNextRound();

        world.AdvanceToNextUnit().ShouldBe(player);
        world.AdvanceToNextUnit().ShouldBe(enemy);
    }

    [Fact]
    public void AdvanceToNextUnit_ShouldReturnNullWhenRoundIsOver()
    {
        ArenaWorld world = TestWorldFactory.CreateWorld(
            TestWorldFactory.CreatePlayer(new Position(0, 0)),
            [TestWorldFactory.CreateEnemy(new Position(1, 0))]);

        world.StartNextRound();
        world.AdvanceToNextUnit();
        world.AdvanceToNextUnit();

        world.AdvanceToNextUnit().ShouldBeNull();
    }

    [Fact]
    public void BeginUnitTurn_ShouldResetActionAndMovePoints()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), moveRange: 3);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, []);

        world.BeginUnitTurn(player);

        player.BattleState.ActionsRemaining.ShouldBe(UnitBattleState.ActionsPerTurn);
        player.BattleState.MovePointsRemaining.ShouldBe(3);
    }

    [Fact]
    public void BeginUnitTurn_ShouldTickPeriodicDamageAndExpireStatuses()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), health: 100);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, []);

        player.BattleState.Statuses.Apply(StatusEffectType.Poison, 2, 10);

        IReadOnlyList<GameActionLogEntry> entries = world.BeginUnitTurn(player);

        player.Stats.GetHealth().ShouldBe(90);
        player.BattleState.Statuses.Has(StatusEffectType.Poison).ShouldBeTrue();

        world.EndUnitTurn(player);

        world.BeginUnitTurn(player);

        player.Stats.GetHealth().ShouldBe(80);

        world.EndUnitTurn(player);

        player.BattleState.Statuses.Has(StatusEffectType.Poison).ShouldBeFalse();
        entries.ShouldNotBeEmpty();
    }

    [Fact]
    public void BeginUnitTurn_StunnedUnit_ShouldLoseTurn()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, []);

        player.BattleState.Statuses.Apply(StatusEffectType.Stun, 1, 1);

        world.BeginUnitTurn(player);

        player.BattleState.ActionsRemaining.ShouldBe(0);
        player.BattleState.MovePointsRemaining.ShouldBe(0);
    }

    [Fact]
    public void BeginUnitTurn_SlowedUnit_ShouldHaveLessMovePoints()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), moveRange: 4);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, []);

        player.BattleState.Statuses.Apply(StatusEffectType.Slow, 3, 2);

        world.BeginUnitTurn(player);

        player.BattleState.MovePointsRemaining.ShouldBe(2);
    }

    [Fact]
    public void IsHostile_PlayerAndEnemy_ShouldBeHostile()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy]);

        world.IsHostile(player, enemy).ShouldBeTrue();
        world.IsHostile(enemy, player).ShouldBeTrue();
        world.IsHostile(player, player).ShouldBeFalse();
    }

    [Fact]
    public void IsHostile_WithExplicitTeams_ShouldCompareTeamIds()
    {
        var teamA = Guid.NewGuid();
        var teamB = Guid.NewGuid();

        PlayerUnit first = new(new Position(0, 0), Domain.GameRuntime.GameActionLogs.FacingDirection.Right)
        {
            CharacterId = Guid.CreateVersion7(),
            Name = "А",
            Spec = Domain.Game.CharacterSpecs.CharacterSpecType.Berserker,
            TeamId = teamA,
            Stats = new GameRuntime.Common.World.Stats.RuntimeStats([]),
            Abilities = new GameRuntime.Common.World.Abilities.RuntimeAbilities([])
        };

        PlayerUnit second = new(new Position(1, 0), Domain.GameRuntime.GameActionLogs.FacingDirection.Right)
        {
            CharacterId = Guid.CreateVersion7(),
            Name = "Б",
            Spec = Domain.Game.CharacterSpecs.CharacterSpecType.Berserker,
            TeamId = teamB,
            Stats = new GameRuntime.Common.World.Stats.RuntimeStats([]),
            Abilities = new GameRuntime.Common.World.Abilities.RuntimeAbilities([])
        };

        EnemyUnit enemy = new(new Position(2, 0), Domain.GameRuntime.GameActionLogs.FacingDirection.Left)
        {
            ArenaEnemyId = Guid.CreateVersion7(),
            EnemyId = Guid.CreateVersion7(),
            Name = "В",
            TeamId = teamA,
            Stats = new GameRuntime.Common.World.Stats.RuntimeStats([]),
            Abilities = new GameRuntime.Common.World.Abilities.RuntimeAbilities([])
        };

        ArenaWorld world = TestWorldFactory.CreateWorld(
            TestWorldFactory.CreatePlayer(new Position(3, 0)),
            [enemy]);

        world.IsHostile(first, second).ShouldBeTrue();
        world.IsHostile(first, enemy).ShouldBeFalse();
    }

    [Fact]
    public void GetOccupant_ShouldFindLivingUnit()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy]);

        world.GetOccupant(new Position(1, 0)).ShouldBe(enemy);
        world.GetOccupant(new Position(2, 2)).ShouldBeNull();
    }
}
