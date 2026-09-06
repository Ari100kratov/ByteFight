using SharedKernel;

namespace Domain.Game.Talents;

/// <summary>
/// Правила прокачки талантов: доступные очки, требования уровней
/// и ступеней ветки, лимиты рангов.
/// </summary>
public static class TalentRules
{
    public static class Errors
    {
        public static Error NotFound(string talentId) =>
            Error.NotFound("talent_not_found", $"Талант '{talentId}' не найден.");

        public static Error RankMaxed(string talentId) =>
            Error.Failure("talent_rank_maxed", $"Ранг таланта '{talentId}' уже максимален.");

        public static Error LevelTooLow(string talentId, int required, int actual) =>
            Error.Failure("talent_level_too_low",
                $"Талант '{talentId}' требует {required} уровень (сейчас {actual}).");

        public static Error TierLocked(string talentId, string branchId, int tier) =>
            Error.Failure("talent_tier_locked",
                $"Для таланта '{talentId}' нужно взять любой талант ступени {tier - 1} ветки '{branchId}'.");

        public static Error NoPoints(int available) =>
            Error.Failure("talent_no_points",
                available == 0
                    ? "Очки талантов исчерпаны."
                    : $"Доступно очков талантов: {available}.");

        public static Error NotForClass(string talentId, string className) =>
            Error.Failure("talent_not_for_class",
                $"Талант '{talentId}' недоступен для класса '{className}'.");
    }

    /// <summary>
    /// Сколько очков талантов даёт уровень (по одному за уровень,
    /// первый уровень очков не даёт).
    /// </summary>
    public static int PointsForLevel(int level) => Math.Max(0, level - 1);

    /// <summary>
    /// Проверяет, можно ли выучить следующий ранг таланта,
    /// и возвращает ошибку с причиной отказа.
    /// </summary>
    /// <param name="tree">Дерево талантов класса персонажа.</param>
    /// <param name="characterLevel">Текущий уровень персонажа.</param>
    /// <param name="chosen">Выбранные таланты: идентификатор → ранг.</param>
    /// <param name="talentId">Идентификатор изучаемого таланта.</param>
    public static Result CanLearn(
        TalentTreeDefinition tree,
        int characterLevel,
        IReadOnlyDictionary<string, int> chosen,
        string talentId)
    {
        TalentNodeDefinition? node = tree.FindNode(talentId);

        if (node is null)
        {
            return Result.Failure(Errors.NotFound(talentId));
        }

        int currentRank = chosen.GetValueOrDefault(talentId);

        if (currentRank >= node.MaxRank)
        {
            return Result.Failure(Errors.RankMaxed(talentId));
        }

        if (characterLevel < node.RequiredLevel)
        {
            return Result.Failure(Errors.LevelTooLow(talentId, node.RequiredLevel, characterLevel));
        }

        if (node.Tier > 1 && !IsPreviousTierTaken(tree, node, chosen))
        {
            return Result.Failure(Errors.TierLocked(talentId, node.BranchId, node.Tier));
        }

        int spent = TotalSpentRanks(chosen);
        int available = PointsForLevel(characterLevel) - spent;

        if (available <= 0)
        {
            return Result.Failure(Errors.NoPoints(Math.Max(0, available)));
        }

        return Result.Success();
    }

    /// <summary>
    /// Сумма всех потраченных рангов.
    /// </summary>
    public static int TotalSpentRanks(IReadOnlyDictionary<string, int> chosen) =>
        chosen.Values.Sum();

    /// <summary>
    /// Проверяет, взят ли хотя бы один ранг любого узла предыдущей ступени
    /// в той же ветке.
    /// </summary>
    public static bool IsPreviousTierTaken(
        TalentTreeDefinition tree,
        TalentNodeDefinition node,
        IReadOnlyDictionary<string, int> chosen)
    {
        if (node.Tier <= 1)
        {
            return true;
        }

        return tree.Nodes
            .Where(x => x.BranchId == node.BranchId && x.Tier == node.Tier - 1)
            .Any(x => chosen.GetValueOrDefault(x.Id) > 0);
    }
}
