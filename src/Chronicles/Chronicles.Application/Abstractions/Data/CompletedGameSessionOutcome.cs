namespace Chronicles.Application.Abstractions.Data;

/// <summary>
/// Исход завершённой игровой сессии в контексте подсистемы хроник.
/// </summary>
public enum CompletedGameSessionOutcome
{
    /// <summary>
    /// Победа игрока.
    /// </summary>
    Victory = 1,

    /// <summary>
    /// Поражение игрока.
    /// </summary>
    Defeat = 2,

    /// <summary>
    /// Ничья.
    /// </summary>
    Draw = 3,

    /// <summary>
    /// Поражение по таймауту исполнения.
    /// </summary>
    TimeoutLoss = 4,

    /// <summary>
    /// Поражение по лимиту ходов.
    /// </summary>
    TurnLimitLoss = 5
}
