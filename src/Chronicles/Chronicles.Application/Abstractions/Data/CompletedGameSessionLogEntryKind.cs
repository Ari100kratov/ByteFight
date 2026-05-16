namespace Chronicles.Application.Abstractions.Data;

/// <summary>
/// Тип записи журнала завершённой игровой сессии.
/// </summary>
public enum CompletedGameSessionLogEntryKind
{
    Idle = 1,
    Walk = 2,
    Death = 3,
    AbilityUsed = 4,
    ItemPickedUp = 5
}
