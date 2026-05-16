namespace Chronicles.Domain;

/// <summary>
/// Тип номинации в системе хроник.
/// </summary>
public enum ChronicleNominationType
{
    /// <summary>
    /// Самая быстрая победа по количеству ходов.
    /// </summary>
    FastestVictory = 1,

    /// <summary>
    /// Самая длинная битва по количеству ходов.
    /// </summary>
    LongestBattle = 2,

    /// <summary>
    /// Наибольшее количество сыгранных боёв.
    /// </summary>
    MostBattlesPlayed = 3,

    /// <summary>
    /// Наибольшее количество побед.
    /// </summary>
    MostVictories = 4,

    /// <summary>
    /// Наибольшее количество поражений.
    /// </summary>
    MostDefeats = 5,

    /// <summary>
    /// Наибольшее количество ничьих.
    /// </summary>
    MostDraws = 6,

    /// <summary>
    /// Наибольший нанесённый урон.
    /// </summary>
    MostDamageDealt = 7,

    /// <summary>
    /// Наибольшее восстановленное здоровье.
    /// </summary>
    MostHealingDone = 8,

    /// <summary>
    /// Наибольшее количество перемещений.
    /// </summary>
    MostMovementActions = 9,

    /// <summary>
    /// Наибольшее количество подобранных предметов.
    /// </summary>
    MostItemsPickedUp = 10,

    /// <summary>
    /// Наибольшее количество пропущенных ходов.
    /// </summary>
    MostIdleTurns = 11,

    /// <summary>
    /// Наибольший полученный урон.
    /// </summary>
    MostDamageTaken = 12
}
