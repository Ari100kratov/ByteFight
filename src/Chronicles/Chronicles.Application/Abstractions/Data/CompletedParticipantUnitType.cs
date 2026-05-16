namespace Chronicles.Application.Abstractions.Data;

/// <summary>
/// Тип участника завершённой игровой сессии.
/// </summary>
public enum CompletedParticipantUnitType
{
    /// <summary>
    /// Персонаж игрока.
    /// </summary>
    Player = 1,

    /// <summary>
    /// Неигровой участник.
    /// </summary>
    Npc = 2
}
