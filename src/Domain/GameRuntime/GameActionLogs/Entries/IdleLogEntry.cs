namespace Domain.GameRuntime.GameActionLogs.Entries;

/// <summary>
/// Запись журнала о пропуске хода.
/// </summary>
public sealed class IdleLogEntry : GameActionLogEntry
{
    private IdleLogEntry() { } // EF

    public IdleLogEntry(
        Guid sessionId,
        UnitId actorId,
        string actorName,
        string? info,
        int turnIndex)
        : base(
            sessionId,
            actorId,
            actorName,
            GameActionLogEntryType.Idle,
            info,
            turnIndex)
    {
    }
}

public static class IdleReasons
{
    public const string ManualIdle =
        "Пропускает ход. Делает вид, что так и было задумано.";

    public static string UserError(string message) =>
        $"Сломался об код: {message}";

    public static string Timeout(TimeSpan timeout) =>
        $"Думал {timeout.TotalSeconds} сек. Ничего не придумал.";

    public const string InvalidAction =
        "Попытался сделать невозможное. Получилось стоять.";

    public const string NoPath =
        "Путь не найден. Энтузиазм тоже.";

    public const string MoveImpossible =
        "Уперся во всё сразу.";

    public const string TargetDead =
        "Цель уже мертва. Неловко вышло.";

    public const string OutOfRange =
        "Не дотягивается. Ни оружием, ни надеждами.";

    public const string NoActionPoints =
        "Очки действия исчерпаны. Ход прожит зря.";

    public const string OnCooldown =
        "Способность ещё отдыхает после прошлого подвига.";

    public const string NotEnoughMana =
        "Сил не хватило. Мана на нуле.";

    public const string InvalidTarget =
        "Цель выбрана странно. Так нельзя.";

    public const string NoTargetsHit =
        "Промах по пустоте. Красиво, но бесполезно.";
}
