namespace Chronicles.Domain;

/// <summary>
/// Агрегированная статистика персонажа для раздела хроник.
/// </summary>
public sealed class CharacterChronicleStats
{
    /// <summary>
    /// Идентификатор записи.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор персонажа.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Отображаемое имя персонажа на момент последней обработки.
    /// </summary>
    public string CharacterName { get; private set; } = string.Empty;

    /// <summary>
    /// Общее число сыгранных битв.
    /// </summary>
    public int BattlesPlayed { get; private set; }

    /// <summary>
    /// Общее число побед.
    /// </summary>
    public int Victories { get; private set; }

    /// <summary>
    /// Общее число поражений.
    /// </summary>
    public int Defeats { get; private set; }

    /// <summary>
    /// Общее число ничьих.
    /// </summary>
    public int Draws { get; private set; }

    /// <summary>
    /// Суммарное количество сыгранных ходов.
    /// </summary>
    public int TotalTurnsPlayed { get; private set; }

    /// <summary>
    /// Время последней обработанной сессии персонажа.
    /// </summary>
    public DateTime? LastSessionAtUtc { get; private set; }

    /// <summary>
    /// Создаёт пустую агрегированную статистику для персонажа.
    /// </summary>
    public static CharacterChronicleStats Create(Guid characterId, string characterName) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            CharacterId = characterId,
            CharacterName = characterName
        };

    /// <summary>
    /// Применяет результат одной завершённой сессии к агрегированной статистике.
    /// </summary>
    public void ApplySession(
        string characterName,
        DateTime sessionEndedAtUtc,
        int totalTurns,
        bool isVictory,
        bool isDraw)
    {
        CharacterName = characterName;
        BattlesPlayed++;
        TotalTurnsPlayed += totalTurns;
        LastSessionAtUtc = sessionEndedAtUtc;

        if (isVictory)
        {
            Victories++;
            return;
        }

        if (isDraw)
        {
            Draws++;
            return;
        }

        Defeats++;
    }
}
