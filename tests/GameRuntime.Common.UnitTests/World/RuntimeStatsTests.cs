using Domain.Game.Stats;
using GameRuntime.Common.World.Stats;
using Shouldly;
using Xunit;

namespace GameRuntime.Common.UnitTests.World;

public sealed class RuntimeStatsTests
{
    [Fact]
    public void ApplyDamage_ShouldNotDropHealthBelowZero()
    {
        var stats = new RuntimeStats([(StatType.Health, 10)]);

        Domain.ValueObjects.StatSnapshot snapshot = stats.ApplyDamage(15);

        stats.GetHealth().ShouldBe(0);
        stats.IsDead().ShouldBeTrue();
        snapshot.Current.ShouldBe(0);
        snapshot.Max.ShouldBe(10);
    }

    [Fact]
    public void Heal_ShouldCapAtMaxHealthAndReturnAppliedValue()
    {
        var stats = new RuntimeStats([(StatType.Health, 10)]);
        stats.ApplyDamage(7);

        StatApplyResult result = stats.Heal(10);

        result.AppliedValue.ShouldBe(7);
        result.Snapshot.Current.ShouldBe(10);
        stats.IsHealthFull().ShouldBeTrue();
    }

    [Fact]
    public void Modify_ShouldClampCurrentValueWhenMaximumDecreases()
    {
        var stats = new RuntimeStats([(StatType.Health, 10)]);

        stats.Modify(StatType.Health, -4);

        stats.GetMaxHealth().ShouldBe(6);
        stats.GetHealth().ShouldBe(6);
    }
}
