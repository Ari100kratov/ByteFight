using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.Turns;
using Shouldly;
using Xunit;

namespace GameRuntime.UnitTests.Logic.Turns;

public sealed class GameTurnProcessorTests
{
    [Fact]
    public async Task ProcessTurn_ShouldProcessPlayerThenEnemiesUntilPlayerDies()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit firstEnemy = TestWorldFactory.CreateEnemy(new Position(1, 0));
        EnemyUnit secondEnemy = TestWorldFactory.CreateEnemy(new Position(2, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [firstEnemy, secondEnemy]);

        var playerProcessor = new FakeUnitTurnProcessor((actor, arena) =>
            [arena.CreateIdleLogEntry(actor, "player")]);
        var npcProcessor = new FakeUnitTurnProcessor((actor, arena) =>
        {
            arena.Player.Stats.ApplyDamage(100);
            arena.Player.MarkKilledBy(actor.Id);
            return [arena.CreateIdleLogEntry(actor, "npc")];
        });

        var processor = new GameTurnProcessor(npcProcessor, playerProcessor);

        Domain.GameRuntime.GameActionLogs.TurnLog turnLog = await processor.ProcessTurn(world);

        world.TurnIndex.ShouldBe(1);
        turnLog.TurnIndex.ShouldBe(1);
        turnLog.Logs.Count.ShouldBe(2);
        playerProcessor.CallCount.ShouldBe(1);
        npcProcessor.CallCount.ShouldBe(1);
        player.IsDead.ShouldBeTrue();
    }

    private sealed class FakeUnitTurnProcessor(
        Func<BaseUnit, ArenaWorld, IReadOnlyList<GameActionLogEntry>> handler)
        : IUnitTurnProcessor
    {
        public int CallCount { get; private set; }

        public IEnumerable<GameActionLogEntry> ProcessTurn(BaseUnit actor, ArenaWorld world)
        {
            CallCount++;
            return handler(actor, world);
        }
    }
}
