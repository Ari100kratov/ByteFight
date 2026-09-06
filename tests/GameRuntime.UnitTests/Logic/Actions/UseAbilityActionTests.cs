using Domain.Game.Abilities;
using Domain.Game.Statuses;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Units;
using GameRuntime.Logic.Actions;
using Shouldly;
using Xunit;

namespace GameRuntime.UnitTests.Logic.Actions;

public sealed class UseAbilityActionTests
{
    [Fact]
    public void Execute_ShouldApplyDamageAndCreateAbilityLog()
    {
        PlayerUnit actor = PrepareActor(out ArenaWorld world, out EnemyUnit target);
        RuntimeAbility ability = CreateDamageAbility(range: 1, damage: 7);

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(actor, ability, target.Position).Execute(world).ToList();

        target.Stats.GetHealth().ShouldBe(13);
        logs.ShouldContain(e => e is AbilityUsedLogEntry);
        AbilityUsedLogEntry abilityLog = logs.OfType<AbilityUsedLogEntry>().Single();
        abilityLog.Value.ShouldBe(7);
        abilityLog.TargetHp.Current.ShouldBe(13);
    }

    [Fact]
    public void Execute_ShouldMarkKilledTargetAndCreateDeathLog()
    {
        PlayerUnit actor = PrepareActor(out ArenaWorld world, out EnemyUnit target, targetHealth: 5);
        RuntimeAbility ability = CreateDamageAbility(range: 1, damage: 10);

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(actor, ability, target.Position).Execute(world).ToList();

        target.IsDead.ShouldBeTrue();
        target.KilledByUnitId.ShouldBe(actor.Id);
        logs.ShouldContain(e => e is AbilityUsedLogEntry);
        logs.ShouldContain(e => e is DeathLogEntry);
    }

    [Fact]
    public void Execute_ShouldReturnIdleWhenTargetIsOutOfRange()
    {
        PlayerUnit actor = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit target = TestWorldFactory.CreateEnemy(new Position(3, 0), health: 20);
        ArenaWorld world = TestWorldFactory.CreateWorld(actor, [target], gridWidth: 6, gridHeight: 5);
        RuntimeAbility ability = CreateDamageAbility(range: 1, damage: 10);
        actor.BattleState.ActionsRemaining = 2;

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(actor, ability, target.Position).Execute(world).ToList();

        target.Stats.GetHealth().ShouldBe(20);
        logs.Single().ShouldBeOfType<IdleLogEntry>();
        logs.Single().Info.ShouldBe(IdleReasons.OutOfRange);
    }

    [Fact]
    public void Execute_ShouldSpendActionPointAndStartCooldown()
    {
        PlayerUnit actor = PrepareActor(out ArenaWorld world, out EnemyUnit target);
        RuntimeAbility ability = CreateDamageAbility(range: 1, damage: 5, cooldown: 3);

        actor.BattleState.ActionsRemaining = 2;

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(actor, ability, target.Position).Execute(world).ToList();

        logs.ShouldNotBeEmpty();
        actor.BattleState.ActionsRemaining.ShouldBe(1);
        actor.BattleState.GetCooldown(ability.Type).ShouldBe(3);
    }

    [Fact]
    public void Execute_ShouldRefuseWithoutActionPoints()
    {
        PlayerUnit actor = PrepareActor(out ArenaWorld world, out EnemyUnit target);
        RuntimeAbility ability = CreateDamageAbility(range: 1, damage: 5);
        actor.BattleState.ActionsRemaining = 0;

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(actor, ability, target.Position).Execute(world).ToList();

        logs.Single().ShouldBeOfType<IdleLogEntry>();
        logs.Single().Info.ShouldBe(IdleReasons.NoActionPoints);
    }

    [Fact]
    public void Execute_ShouldRefuseAbilityOnCooldown()
    {
        PlayerUnit actor = PrepareActor(out ArenaWorld world, out EnemyUnit target);
        RuntimeAbility ability = CreateDamageAbility(range: 1, damage: 5);
        actor.BattleState.ActionsRemaining = 2;
        actor.BattleState.StartCooldown(ability.Type, 2);

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(actor, ability, target.Position).Execute(world).ToList();

        logs.Single().ShouldBeOfType<IdleLogEntry>();
        logs.Single().Info.ShouldBe(IdleReasons.OnCooldown);
    }

    [Fact]
    public void Execute_AreaAbility_ShouldHitAllEnemiesInRadius()
    {
        PlayerUnit actor = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit first = TestWorldFactory.CreateEnemy(new Position(2, 0), health: 30);
        EnemyUnit second = TestWorldFactory.CreateEnemy(new Position(2, 1), health: 30);
        EnemyUnit farAway = TestWorldFactory.CreateEnemy(new Position(4, 3), health: 30);
        ArenaWorld world = TestWorldFactory.CreateWorld(actor, [first, second, farAway], gridWidth: 6, gridHeight: 6);

        RuntimeAbility ability = new()
        {
            Type = AbilityType.VolkhvAshCloud,
            EffectType = AbilityEffectType.Damage,
            TargetType = AbilityTargetType.Area,
            Shape = AbilityShape.Radius,
            Name = "Пепельный туман",
            Stats = new Dictionary<AbilityStatType, decimal>
            {
                [AbilityStatType.Range] = 4,
                [AbilityStatType.AreaRadius] = 1,
                [AbilityStatType.Damage] = 10
            }
        };

        actor.BattleState.ActionsRemaining = 2;

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(actor, ability, first.Position).Execute(world).ToList();

        first.Stats.GetHealth().ShouldBe(20);
        second.Stats.GetHealth().ShouldBe(20);
        farAway.Stats.GetHealth().ShouldBe(30);
        logs.OfType<AbilityUsedLogEntry>().Count().ShouldBe(2);
    }

    [Fact]
    public void Execute_WithStatuses_ShouldApplyStatusToTarget()
    {
        PlayerUnit actor = PrepareActor(out ArenaWorld world, out EnemyUnit target);
        RuntimeAbility ability = new()
        {
            Type = AbilityType.HunterPoisonArrow,
            EffectType = AbilityEffectType.Damage,
            TargetType = AbilityTargetType.Enemy,
            Shape = AbilityShape.SingleTarget,
            Name = "Ядовитая стрела",
            Stats = new Dictionary<AbilityStatType, decimal>
            {
                [AbilityStatType.Range] = 1,
                [AbilityStatType.Damage] = 5
            },
            AppliedStatuses = [new AbilityStatusEffect(StatusEffectType.Poison, 3, 8)]
        };

        actor.BattleState.ActionsRemaining = 2;

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(actor, ability, target.Position).Execute(world).ToList();

        target.BattleState.Statuses.Has(StatusEffectType.Poison).ShouldBeTrue();
        logs.ShouldContain(e => e is StatusAppliedLogEntry);
    }

    [Fact]
    public void Execute_SelfBuff_ShouldApplyStatusesToActor()
    {
        PlayerUnit actor = TestWorldFactory.CreatePlayer(new Position(0, 0));
        ArenaWorld world = TestWorldFactory.CreateWorld(actor, []);

        RuntimeAbility ability = new()
        {
            Type = AbilityType.VityazBulwark,
            EffectType = AbilityEffectType.Buff,
            TargetType = AbilityTargetType.Self,
            Shape = AbilityShape.SingleTarget,
            Name = "Стойка богатыря",
            Stats = new Dictionary<AbilityStatType, decimal>(),
            AppliedStatuses = [new AbilityStatusEffect(StatusEffectType.Shield, 3, 40)]
        };

        actor.BattleState.ActionsRemaining = 2;

        IReadOnlyList<GameActionLogEntry> logs =
            new UseAbilityAction(actor, ability, actor.Position).Execute(world).ToList();

        actor.BattleState.Statuses.Has(StatusEffectType.Shield).ShouldBeTrue();
        logs.OfType<StatusAppliedLogEntry>().Count().ShouldBe(1);
    }

    /// <summary>
    /// Атакующий стоит рядом с целью, у него есть очки действия.
    /// </summary>
    private static PlayerUnit PrepareActor(
        out ArenaWorld world,
        out EnemyUnit target,
        decimal targetHealth = 20)
    {
        PlayerUnit actor = TestWorldFactory.CreatePlayer(new Position(0, 0));
        target = TestWorldFactory.CreateEnemy(new Position(1, 0), health: targetHealth);
        world = TestWorldFactory.CreateWorld(actor, [target], gridWidth: 6, gridHeight: 5);
        actor.BattleState.ActionsRemaining = 2;

        return actor;
    }

    private static RuntimeAbility CreateDamageAbility(int range, decimal damage, int cooldown = 0) =>
        new()
        {
            Type = AbilityType.BasicMeleeAttack,
            EffectType = AbilityEffectType.Damage,
            TargetType = AbilityTargetType.Enemy,
            Name = "Верный удар",
            Stats = new Dictionary<AbilityStatType, decimal>
            {
                [AbilityStatType.Range] = range,
                [AbilityStatType.Damage] = damage,
                [AbilityStatType.Cooldown] = cooldown
            }
        };
}
