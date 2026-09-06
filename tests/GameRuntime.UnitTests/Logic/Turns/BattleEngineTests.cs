using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.Turns;
using GameRuntime.Realtime;
using Shouldly;
using Xunit;

namespace GameRuntime.UnitTests.Logic.Turns;

public sealed class BattleEngineTests
{
    [Fact]
    public async Task StepAsync_FirstStep_ShouldStartRound()
    {
        ArenaWorld world = CreateBattleWorld();
        BattleEngine engine = CreateEngine(world);

        BattleStepResult step = await engine.StepAsync(CancellationToken.None);

        world.RoundNumber.ShouldBe(1);
        step.Logs.ShouldContain(e => e is RoundStartedLogEntry);
        step.Finished.ShouldBeFalse();
    }

    [Fact]
    public async Task StepAsync_ManualPlayerTurn_ShouldWaitForCommands()
    {
        ArenaWorld world = CreateBattleWorld();
        BattleCommandQueue queue = new();
        BattleEngine engine = CreateEngine(world, queue);

        // Первый шаг — старт раунда.
        await engine.StepAsync(CancellationToken.None);

        // Второй шаг — ход игрока: ожидает команду.
        Task<BattleStepResult> playerTurn = engine.StepAsync(CancellationToken.None);

        // Даём движку время дойти до ожидания команды.
        await Task.Delay(50);
        playerTurn.IsCompleted.ShouldBeFalse();

        // Команда завершения хода закрывает ожидание.
        queue.TryPost(new EndTurnCommand());

        BattleStepResult result = await playerTurn;

        result.Finished.ShouldBeFalse();
        result.Logs.ShouldBeEmpty();
    }

    [Fact]
    public async Task StepAsync_PlayerKillsAllEnemies_ShouldFinish()
    {
        // Игрок с инициативой 10 ходит первым и убивает врага одним ударом.
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), initiative: 10);
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0), health: 5, initiative: 1);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy], gridWidth: 6, gridHeight: 5);

        SetupBasicAttack(player);

        BattleCommandQueue queue = new();
        BattleEngine engine = CreateEngine(world, queue);

        await engine.StepAsync(CancellationToken.None); // старт раунда

        Task<BattleStepResult> playerTurn = engine.StepAsync(CancellationToken.None);
        await Task.Delay(50);

        queue.TryPost(new UseAbilityCommand(
            Domain.Game.Abilities.AbilityType.BasicMeleeAttack,
            enemy.Position));

        await playerTurn;

        enemy.IsDead.ShouldBeTrue();

        // Следующий шаг: враг мёртв, раунд должен завершить бой.
        BattleStepResult finish = await engine.StepAsync(CancellationToken.None);
        finish.Finished.ShouldBeTrue();
    }

    [Fact]
    public async Task StepAsync_NpcTurn_ShouldRunAutomatically()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), initiative: 1);
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(1, 0), initiative: 10);
        ArenaWorld world = TestWorldFactory.CreateWorld(player, [enemy], gridWidth: 6, gridHeight: 5);

        // Врагу нужна атака, чтобы походить осмысленно.
        SetupBasicAttack(enemy);

        BattleEngine engine = CreateEngine(world, new BattleCommandQueue());

        await engine.StepAsync(CancellationToken.None); // старт раунда

        // Ход врага (инициатива выше) выполняется без ожидания команд.
        BattleStepResult npcTurn = await engine.StepAsync(CancellationToken.None);

        world.ActiveUnit.ShouldBe(enemy);
        npcTurn.Logs.ShouldNotBeEmpty();
    }

    private static void SetupBasicAttack(BaseUnit unit)
    {
        Domain.Game.Abilities.AbilityDefinition definition =
            Domain.Game.Abilities.AbilityCatalog.Get(Domain.Game.Abilities.AbilityType.BasicMeleeAttack);

        var ability = new GameRuntime.Common.World.Abilities.RuntimeAbility
        {
            Type = definition.Type,
            EffectType = definition.EffectType,
            TargetType = definition.TargetType,
            Shape = definition.Shape,
            Name = definition.Name,
            Stats = definition.Stats,
            AppliedStatuses = definition.Statuses ?? []
        };

        unit.Abilities.Add(ability);
    }

    private static ArenaWorld CreateBattleWorld()
    {
        PlayerUnit player = TestWorldFactory.CreatePlayer(new Position(0, 0), initiative: 10);
        EnemyUnit enemy = TestWorldFactory.CreateEnemy(new Position(2, 0), initiative: 5);

        return TestWorldFactory.CreateWorld(player, [enemy], gridWidth: 6, gridHeight: 5);
    }

    private static BattleEngine CreateEngine(ArenaWorld world, BattleCommandQueue queue) =>
        new(
            world,
            new ScriptedNpcRunner(),
            new ManualPlayerTurnRunner(queue),
            broadcastAction: null);

    private static BattleEngine CreateEngine(ArenaWorld world) =>
        new(
            world,
            new ScriptedNpcRunner(),
            new ScriptedNpcRunner(),
            broadcastAction: null);

    /// <summary>
    /// Простой NPC-раннер для тестов: помечает, что юнит походил.
    /// </summary>
    private sealed class ScriptedNpcRunner : IBattleTurnRunner
    {
        public Task<IReadOnlyList<GameActionLogEntry>> PlayTurn(
            BaseUnit unit,
            ArenaWorld world,
            Action<IReadOnlyList<GameActionLogEntry>> onActionPerformed,
            CancellationToken ct)
        {
            unit.BattleState.ActionsRemaining = 0;
            unit.BattleState.MovePointsRemaining = 0;

            IReadOnlyList<GameActionLogEntry> logs = [world.CreateIdleLogEntry(unit, "Ход сделан")];
            onActionPerformed(logs);

            return Task.FromResult(logs);
        }
    }
}
