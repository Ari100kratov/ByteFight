namespace Chronicles.Domain;

/// <summary>
/// Текущий результат персонажа в одной номинации.
/// </summary>
public sealed class CharacterNominationScore
{
    /// <summary>
    /// Идентификатор результата.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор персонажа.
    /// </summary>
    public Guid CharacterId { get; private set; }

    /// <summary>
    /// Отображаемое имя персонажа на момент последнего обновления результата.
    /// </summary>
    public string CharacterName { get; private set; } = string.Empty;

    /// <summary>
    /// Имя владельца персонажа на момент последнего обновления результата.
    /// </summary>
    public string? UserFirstName { get; private set; }

    /// <summary>
    /// Фамилия владельца персонажа на момент последнего обновления результата.
    /// </summary>
    public string? UserLastName { get; private set; }

    /// <summary>
    /// Название класса персонажа на момент последнего обновления результата.
    /// </summary>
    public string? CharacterClassName { get; private set; }

    /// <summary>
    /// Название специализации персонажа на момент последнего обновления результата.
    /// </summary>
    public string? CharacterSpecName { get; private set; }

    /// <summary>
    /// Тип номинации.
    /// </summary>
    public ChronicleNominationType NominationType { get; private set; }

    /// <summary>
    /// Текущее значение результата.
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// Сессия, которая сформировала текущее значение или последнее накопительное изменение.
    /// </summary>
    public Guid? SessionId { get; private set; }

    /// <summary>
    /// Момент события, сформировавшего текущее значение.
    /// </summary>
    public DateTime? OccurredAtUtc { get; private set; }

    /// <summary>
    /// Момент последнего обновления read-model.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Создаёт пустой результат персонажа в номинации.
    /// </summary>
    public static CharacterNominationScore Create(
        Guid characterId,
        string characterName,
        ChronicleNominationType nominationType,
        DateTime updatedAtUtc) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            CharacterId = characterId,
            CharacterName = characterName,
            NominationType = nominationType,
            UpdatedAtUtc = updatedAtUtc
        };

    /// <summary>
    /// Добавляет накопительное значение к результату.
    /// </summary>
    public void Add(decimal value, string characterName, Guid? sessionId, DateTime occurredAtUtc, DateTime updatedAtUtc)
    {
        Value += value;
        UpdateSource(characterName, null, null, null, null, sessionId, occurredAtUtc, updatedAtUtc);
    }

    /// <summary>
    /// Добавляет накопительное значение к результату и обновляет отображаемые данные участника.
    /// </summary>
    public void Add(
        decimal value,
        string characterName,
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        Guid? sessionId,
        DateTime occurredAtUtc,
        DateTime updatedAtUtc)
    {
        Value += value;
        UpdateSource(characterName, userFirstName, userLastName, characterClassName, characterSpecName, sessionId, occurredAtUtc, updatedAtUtc);
    }

    /// <summary>
    /// Заменяет результат, если новое значение больше текущего.
    /// </summary>
    public void ReplaceIfGreater(
        decimal value,
        string characterName,
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        Guid? sessionId,
        DateTime occurredAtUtc,
        DateTime updatedAtUtc)
    {
        if (Value >= value)
        {
            UpdateMetadata(characterName, userFirstName, userLastName, characterClassName, characterSpecName, updatedAtUtc);
            return;
        }

        Value = value;
        UpdateSource(characterName, userFirstName, userLastName, characterClassName, characterSpecName, sessionId, occurredAtUtc, updatedAtUtc);
    }

    /// <summary>
    /// Заменяет результат, если новое значение больше текущего.
    /// </summary>
    public void ReplaceIfGreater(decimal value, string characterName, Guid? sessionId, DateTime occurredAtUtc, DateTime updatedAtUtc) =>
        ReplaceIfGreater(value, characterName, null, null, null, null, sessionId, occurredAtUtc, updatedAtUtc);

    /// <summary>
    /// Заменяет результат, если новое значение меньше текущего или результат ещё пустой.
    /// </summary>
    public void ReplaceIfLower(
        decimal value,
        string characterName,
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        Guid? sessionId,
        DateTime occurredAtUtc,
        DateTime updatedAtUtc)
    {
        if (Value > 0 && Value <= value)
        {
            UpdateMetadata(characterName, userFirstName, userLastName, characterClassName, characterSpecName, updatedAtUtc);
            return;
        }

        Value = value;
        UpdateSource(characterName, userFirstName, userLastName, characterClassName, characterSpecName, sessionId, occurredAtUtc, updatedAtUtc);
    }

    /// <summary>
    /// Заменяет результат, если новое значение меньше текущего или результат ещё пустой.
    /// </summary>
    public void ReplaceIfLower(decimal value, string characterName, Guid? sessionId, DateTime occurredAtUtc, DateTime updatedAtUtc) =>
        ReplaceIfLower(value, characterName, null, null, null, null, sessionId, occurredAtUtc, updatedAtUtc);

    /// <summary>
    /// Обновляет отображаемые данные участника без изменения значения результата.
    /// </summary>
    public void UpdateParticipantMetadata(
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        DateTime updatedAtUtc) =>
        UpdateMetadata(CharacterName, userFirstName, userLastName, characterClassName, characterSpecName, updatedAtUtc);

    private void UpdateSource(
        string characterName,
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        Guid? sessionId,
        DateTime occurredAtUtc,
        DateTime updatedAtUtc)
    {
        UpdateMetadata(characterName, userFirstName, userLastName, characterClassName, characterSpecName, updatedAtUtc);
        SessionId = sessionId;
        OccurredAtUtc = occurredAtUtc;
    }

    private void UpdateMetadata(
        string characterName,
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        DateTime updatedAtUtc)
    {
        CharacterName = characterName;
        UserFirstName = string.IsNullOrWhiteSpace(userFirstName) ? UserFirstName : userFirstName;
        UserLastName = string.IsNullOrWhiteSpace(userLastName) ? UserLastName : userLastName;
        CharacterClassName = string.IsNullOrWhiteSpace(characterClassName) ? CharacterClassName : characterClassName;
        CharacterSpecName = string.IsNullOrWhiteSpace(characterSpecName) ? CharacterSpecName : characterSpecName;
        UpdatedAtUtc = updatedAtUtc;
    }
}
