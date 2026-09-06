namespace Domain.Game.Leveling;

/// <summary>
/// Правила опыта и уровней персонажа.
/// </summary>
public static class LevelingRules
{
    /// <summary>
    /// Максимальный уровень персонажа.
    /// </summary>
    public const int MaxLevel = 30;

    /// <summary>
    /// Возвращает опыт, необходимый для перехода с уровня
    /// <paramref name="level"/> на следующий.
    /// </summary>
    public static int ExperienceToNextLevel(int level) =>
        level >= MaxLevel ? int.MaxValue : 100 + 60 * (level - 1);

    /// <summary>
    /// Добавляет опыт и возвращает итоговые уровень и опыт внутри уровня.
    /// Не превышает максимальный уровень.
    /// </summary>
    public static (int Level, int Experience, int LevelsGained) AddExperience(
        int level,
        int experience,
        int gained)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(gained);

        int startLevel = level;
        experience += gained;

        while (level < MaxLevel && experience >= ExperienceToNextLevel(level))
        {
            experience -= ExperienceToNextLevel(level);
            level++;
        }

        if (level >= MaxLevel)
        {
            experience = 0;
        }

        return (level, experience, level - startLevel);
    }
}
