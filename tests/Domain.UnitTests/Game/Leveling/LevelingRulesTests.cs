using Domain.Game.Leveling;
using Shouldly;
using Xunit;

namespace Domain.UnitTests.Game.Leveling;

public sealed class LevelingRulesTests
{
    [Fact]
    public void ExperienceToNextLevel_ShouldGrowWithLevel()
    {
        LevelingRules.ExperienceToNextLevel(1).ShouldBe(100);
        LevelingRules.ExperienceToNextLevel(2).ShouldBe(160);
        LevelingRules.ExperienceToNextLevel(3).ShouldBe(220);
    }

    [Fact]
    public void AddExperience_BelowThreshold_ShouldKeepLevel()
    {
        (int level, int experience, int gained) = LevelingRules.AddExperience(1, 0, 99);

        level.ShouldBe(1);
        experience.ShouldBe(99);
        gained.ShouldBe(0);
    }

    [Fact]
    public void AddExperience_AboveThreshold_ShouldLevelUp()
    {
        (int level, int experience, int gained) = LevelingRules.AddExperience(1, 50, 100);

        level.ShouldBe(2);
        experience.ShouldBe(50); // 150 - 100
        gained.ShouldBe(1);
    }

    [Fact]
    public void AddExperience_MultipleLevels_ShouldChain()
    {
        (int level, _, int gained) = LevelingRules.AddExperience(1, 0, 1000);

        // 100 + 160 + 220 + 280 + 340 = 1100 > 1000 → пять уровней.
        level.ShouldBe(5);
        gained.ShouldBe(4);
    }

    [Fact]
    public void AddExperience_AtMaxLevel_ShouldCap()
    {
        (int level, int experience, int _) = LevelingRules.AddExperience(LevelingRules.MaxLevel, 50, 999999);

        level.ShouldBe(LevelingRules.MaxLevel);
        experience.ShouldBe(0);
    }
}
