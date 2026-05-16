namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions.Nominations;

/// <summary>
/// Способ применения нового значения к текущему результату номинации.
/// </summary>
internal enum ChronicleNominationScoreUpdateMode
{
    /// <summary>
    /// Добавить значение к текущему результату.
    /// </summary>
    Increment = 1,

    /// <summary>
    /// Заменить результат, если новое значение больше.
    /// </summary>
    ReplaceIfGreater = 2,

    /// <summary>
    /// Заменить результат, если новое значение меньше.
    /// </summary>
    ReplaceIfLower = 3
}
