namespace Domain.GameRuntime.GameActionLogs;

/// <summary>
/// Тип записи журнала боя.
/// </summary>
public enum GameActionLogEntryType
{
    /// <summary>
    /// Юнит пропустил ход.
    /// </summary>
    Idle = 1,

    /// <summary>
    /// Юнит переместился.
    /// </summary>
    Walk = 2,

    /// <summary>
    /// Юнит применил способность.
    /// </summary>
    AbilityUsed = 3,

    /// <summary>
    /// Юнит погиб.
    /// </summary>
    Death = 4
}
