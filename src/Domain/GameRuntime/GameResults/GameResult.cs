namespace Domain.GameRuntime.GameResults;

/// <summary>
/// Итог завершённой игровой сессии и, если применимо, победивший юнит.
/// </summary>
public class GameResult
{
    /// <summary>
    /// Тип исхода боя.
    /// </summary>
    public GameOutcome Outcome { get; private set; }

    /// <summary>
    /// Победивший юнит для победы игрока или противника; для ничьих и лимитов отсутствует.
    /// </summary>
    public UnitId? WinnerUnitId { get; private set; }

    private GameResult() { } // нужен EF

    private GameResult(GameOutcome outcome, UnitId? winnerUnitId)
    {
        Outcome = outcome;
        WinnerUnitId = winnerUnitId;
    }

    /// <summary>
    /// Создаёт результат победы игрока.
    /// </summary>
    public static GameResult PlayerVictory(Guid characterId) => new(GameOutcome.Victory, new UnitId(characterId));

    /// <summary>
    /// Создаёт результат победы NPC-противника.
    /// </summary>
    public static GameResult EnemyVictory(Guid arenaEnemyId) => new(GameOutcome.Defeat, new UnitId(arenaEnemyId));

    /// <summary>
    /// Создаёт результат ничьей.
    /// </summary>
    public static GameResult Draw() => new(GameOutcome.Draw, null);

    /// <summary>
    /// Создаёт результат поражения по таймауту пользовательского кода.
    /// </summary>
    public static GameResult TimeoutLoss() => new(GameOutcome.TimeoutLoss, null);

    /// <summary>
    /// Создаёт результат поражения по лимиту ходов.
    /// </summary>
    public static GameResult TurnLimitLoss() => new(GameOutcome.TurnLimitLoss, null);
}
