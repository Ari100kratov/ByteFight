using Domain.Game.CharacterClasses;
using Domain.Game.Talents;
using SharedKernel;
using Shouldly;
using Xunit;

namespace Domain.UnitTests.Game.Talents;

public sealed class TalentCatalogTests
{
    [Fact]
    public void AllTrees_ShouldHaveTwoBranchesPerClass()
    {
        foreach (TalentTreeDefinition tree in TalentCatalog.AllTrees())
        {
            tree.Branches.Count.ShouldBeGreaterThanOrEqualTo(2,
                $"Класс {tree.Class} должен иметь минимум две ветки");
        }
    }

    [Fact]
    public void Nodes_ShouldHaveGloballyUniqueIds()
    {
        var ids = TalentCatalog.AllTrees()
            .SelectMany(tree => tree.Nodes)
            .Select(node => node.Id)
            .ToList();

        ids.Distinct().Count().ShouldBe(ids.Count);
    }

    [Fact]
    public void Nodes_ShouldReferenceExistingBranches()
    {
        foreach (TalentTreeDefinition tree in TalentCatalog.AllTrees())
        {
            foreach (TalentNodeDefinition node in tree.Nodes)
            {
                tree.Branches.Select(b => b.Id).ShouldContain(node.BranchId,
                    $"Узел {node.Id} ссылается на неизвестную ветку");
            }
        }
    }

    [Fact]
    public void Nodes_ShouldHaveValidRanksAndTiers()
    {
        foreach (TalentNodeDefinition node in TalentCatalog.AllTrees().SelectMany(t => t.Nodes))
        {
            node.MaxRank.ShouldBeInRange(1, 3);
            node.Tier.ShouldBeInRange(1, 3);
        }
    }

    [Fact]
    public void ActiveNodes_ShouldGrantAbilities()
    {
        foreach (TalentNodeDefinition node in TalentCatalog.AllTrees()
                     .SelectMany(t => t.Nodes)
                     .Where(n => n.NodeType == TalentNodeType.Active))
        {
            node.Bonuses.ShouldContain(
                b => b.Type == TalentBonusType.GrantsAbility,
                $"Активный узел {node.Id} должен открывать способность");
        }
    }

    [Fact]
    public void EveryTree_ShouldHaveActiveCapstone()
    {
        foreach (TalentTreeDefinition tree in TalentCatalog.AllTrees())
        {
            tree.Nodes.Count(n => n.NodeType == TalentNodeType.Active).ShouldBeGreaterThanOrEqualTo(1);
        }
    }

    [Fact]
    public void FindNode_ShouldSearchAcrossTrees()
    {
        TalentCatalog.FindNode("vityaz_sword_perun_wrath").ShouldNotBeNull();
        TalentCatalog.FindNode("missing_talent").ShouldBeNull();
    }
}

public sealed class TalentRulesTests
{
    private static readonly TalentTreeDefinition Tree = TalentCatalog.GetTree(CharacterClassType.Vityaz);

    [Fact]
    public void PointsForLevel_FirstLevel_ShouldGiveNoPoints()
    {
        TalentRules.PointsForLevel(1).ShouldBe(0);
        TalentRules.PointsForLevel(2).ShouldBe(1);
        TalentRules.PointsForLevel(10).ShouldBe(9);
    }

    [Fact]
    public void CanLearn_UnknownTalent_ShouldFail()
    {
        Result result = TalentRules.CanLearn(Tree, 5, new Dictionary<string, int>(), "no_such_talent");

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("talent_not_found");
    }

    [Fact]
    public void CanLearn_TierOneAtLevelTwo_ShouldSucceed()
    {
        var chosen = new Dictionary<string, int>();

        Result result = TalentRules.CanLearn(Tree, 2, chosen, "vityaz_sword_steady_hand");

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void CanLearn_TierOneAtLevelOne_ShouldFailWithLevel()
    {
        Result result = TalentRules.CanLearn(Tree, 1, new Dictionary<string, int>(), "vityaz_sword_steady_hand");

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("talent_level_too_low");
    }

    [Fact]
    public void CanLearn_SecondTierWithoutFirstTier_ShouldFailWithTierLock()
    {
        Result result = TalentRules.CanLearn(Tree, 10, new Dictionary<string, int>(), "vityaz_sword_wild_swing");

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("talent_tier_locked");
    }

    [Fact]
    public void CanLearn_SecondTierAfterFirstTier_ShouldSucceed()
    {
        var chosen = new Dictionary<string, int> { ["vityaz_sword_tempered"] = 1 };

        Result result = TalentRules.CanLearn(Tree, 4, chosen, "vityaz_sword_wild_swing");

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void CanLearn_BeyondMaxRank_ShouldFail()
    {
        var chosen = new Dictionary<string, int> { ["vityaz_sword_steady_hand"] = 2 };

        Result result = TalentRules.CanLearn(Tree, 10, chosen, "vityaz_sword_steady_hand");

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("talent_rank_maxed");
    }

    [Fact]
    public void CanLearn_WithoutPoints_ShouldFail()
    {
        // Уровень 2 даёт одно очко, одно уже потрачено.
        var chosen = new Dictionary<string, int> { ["vityaz_sword_steady_hand"] = 1 };

        Result result = TalentRules.CanLearn(Tree, 2, chosen, "vityaz_sword_tempered");

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("talent_no_points");
    }

    [Fact]
    public void TotalSpentRanks_ShouldSumAllRanks()
    {
        var chosen = new Dictionary<string, int>
        {
            ["a"] = 2,
            ["b"] = 1
        };

        TalentRules.TotalSpentRanks(chosen).ShouldBe(3);
    }
}
