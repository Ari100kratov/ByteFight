namespace Domain.GameRuntime.GameResults;

public class GameResult
{
    public GameOutcome Outcome { get; private set; }
    public UnitId? WinnerUnitId { get; private set; }

    private GameResult() { } // нужен EF

    private GameResult(GameOutcome outcome, UnitId? winnerUnitId)
    {
        Outcome = outcome;
        WinnerUnitId = winnerUnitId;
    }

    public static GameResult PlayerVictory(Guid characterId) => new(GameOutcome.Victory, new UnitId(characterId));

    public static GameResult EnemyVictory(Guid arenaEnemyId) => new(GameOutcome.Defeat, new UnitId(arenaEnemyId));

    public static GameResult Draw() => new(GameOutcome.Draw, null);

    public static GameResult TimeoutLoss() => new(GameOutcome.TimeoutLoss, null);

    public static GameResult TurnLimitLoss() => new(GameOutcome.TurnLimitLoss, null);
}
