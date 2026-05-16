namespace Chronicles.Application.Abstractions.Data;

/// <summary>
/// Исход завершённой игровой сессии в контексте подсистемы хроник.
/// </summary>
public enum CompletedGameSessionOutcome
{
    Victory = 1,
    Defeat = 2,
    Draw = 3,
    TimeoutLoss = 4,
    TurnLimitLoss = 5
}
