using Domain.Game.Abilities;
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
        PlayerUnit actor = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit target = TestWorldFactory.CreateEnemy(new Position(1, 0), health: 20);
        ArenaWorld world = TestWorldFactory.CreateWorld(actor, [target]);
        RuntimeAbility ability = CreateDamageAbility(range: 1, damage: 7);

        IReadOnlyList<GameActionLogEntry> logs = new UseAbilityAction(actor, target, ability)
            .Execute(world);

        target.Stats.GetHealth().ShouldBe(13);
        logs.Single().ShouldBeOfType<AbilityUsedLogEntry>();
        var abilityLog = (AbilityUsedLogEntry)logs.Single();
        abilityLog.Value.ShouldBe(7);
        abilityLog.TargetHp.Current.ShouldBe(13);
    }

    [Fact]
    public void Execute_ShouldMarkKilledTargetAndCreateDeathLog()
    {
        PlayerUnit actor = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit target = TestWorldFactory.CreateEnemy(new Position(1, 0), health: 5);
        ArenaWorld world = TestWorldFactory.CreateWorld(actor, [target]);
        RuntimeAbility ability = CreateDamageAbility(range: 1, damage: 10);

        IReadOnlyList<GameActionLogEntry> logs = new UseAbilityAction(actor, target, ability)
            .Execute(world);

        target.IsDead.ShouldBeTrue();
        target.KilledByUnitId.ShouldBe(actor.Id);
        logs.Count.ShouldBe(2);
        logs[0].ShouldBeOfType<AbilityUsedLogEntry>();
        logs[1].ShouldBeOfType<DeathLogEntry>();
    }

    [Fact]
    public void Execute_ShouldReturnIdleWhenTargetIsOutOfRange()
    {
        PlayerUnit actor = TestWorldFactory.CreatePlayer(new Position(0, 0));
        EnemyUnit target = TestWorldFactory.CreateEnemy(new Position(3, 0), health: 20);
        ArenaWorld world = TestWorldFactory.CreateWorld(actor, [target]);
        RuntimeAbility ability = CreateDamageAbility(range: 1, damage: 10);

        IReadOnlyList<GameActionLogEntry> logs = new UseAbilityAction(actor, target, ability)
            .Execute(world);

        target.Stats.GetHealth().ShouldBe(20);
        logs.Single().ShouldBeOfType<IdleLogEntry>();
        logs.Single().Info.ShouldBe(IdleReasons.OutOfRange);
    }

    [Fact]
    public void Execute_ShouldCapHealingAtTargetMaxHealth()
    {
        PlayerUnit actor = TestWorldFactory.CreatePlayer(new Position(0, 0), health: 20);
        ArenaWorld world = TestWorldFactory.CreateWorld(actor, []);
        actor.Stats.ApplyDamage(6);

        RuntimeAbility ability = CreateHealingAbility(range: 0, healing: 10);

        IReadOnlyList<GameActionLogEntry> logs = new UseAbilityAction(actor, actor, ability)
            .Execute(world);

        actor.Stats.GetHealth().ShouldBe(20);
        AbilityUsedLogEntry abilityLog = logs.Single().ShouldBeOfType<AbilityUsedLogEntry>();
        abilityLog.Value.ShouldBe(6);
        abilityLog.TargetHp.Current.ShouldBe(20);
    }

    private static RuntimeAbility CreateDamageAbility(int range, decimal damage) =>
        new()
        {
            Type = AbilityType.BasicMeleeAttack,
            EffectType = AbilityEffectType.Damage,
            TargetType = AbilityTargetType.Enemy,
            Name = "Strike",
            Stats = new Dictionary<AbilityStatType, decimal>
            {
                [AbilityStatType.Range] = range,
                [AbilityStatType.Damage] = damage
            }
        };

    private static RuntimeAbility CreateHealingAbility(int range, decimal healing) =>
        new()
        {
            Type = AbilityType.Healing,
            EffectType = AbilityEffectType.Healing,
            TargetType = AbilityTargetType.Ally,
            Name = "Heal",
            Stats = new Dictionary<AbilityStatType, decimal>
            {
                [AbilityStatType.Range] = range,
                [AbilityStatType.Healing] = healing
            }
        };
}
