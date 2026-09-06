namespace Chronicles.Application.Abstractions.Data;

/// <summary>
/// Тип записи журнала завершённой игровой сессии.
/// </summary>
public enum CompletedGameSessionLogEntryKind
{
    /// <summary>
    /// Пропуск хода.
    /// </summary>
    Idle = 1,

    /// <summary>
    /// Перемещение.
    /// </summary>
    Walk = 2,

    /// <summary>
    /// Смерть юнита.
    /// </summary>
    Death = 3,

    /// <summary>
    /// Использование способности.
    /// </summary>
    AbilityUsed = 4,

    /// <summary>
    /// Подбор предмета.
    /// </summary>
    ItemPickedUp = 5,

    /// <summary>
    /// Наложение статус-эффекта.
    /// </summary>
    StatusApplied = 6,

    /// <summary>
    /// Начало раунда.
    /// </summary>
    RoundStarted = 7
}
