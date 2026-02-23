using Domain.Game.Actions;

namespace Domain.GameRuntime.GameActionLogs;

public sealed class IdleLogEntry : GameActionLogEntry
{
    private IdleLogEntry() { } // EF

    public IdleLogEntry(
        Guid sessionId,
        Guid actorId,
        string? info,
        int turnIndex)
        : base(sessionId, actorId, ActionType.Idle, info, turnIndex)
    {
    }
}

public static class IdleReasons
{
    public const string ManualIdle =
        "Пропускает ход и оценивает ситуацию.";

    public static string UserError(string message) =>
        $"Что-то пошло не так. Очень не так: {message}";

    public const string InvalidAction =
        "Попытался сделать что-то странное. В итоге просто стоит.";

    public const string NoPath =
        "Смотрит в сторону цели, но пути не видит.";

    public const string MoveImpossible =
        "Пытается двинуться, но вокруг одни препятствия.";

    public const string TargetDead =
        "Собирался атаковать… но цель уже повержена.";

    public const string OutOfRange =
        "Размахнулся, но противник слишком далеко.";
}
