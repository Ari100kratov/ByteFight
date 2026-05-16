namespace Chronicles.Domain;

/// <summary>
/// Фиксирует достижение персонажа в рамках конкретной номинации.
/// </summary>
public sealed class ChronicleRecord
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
    /// Отображаемое имя персонажа на момент достижения.
    /// </summary>
    public string CharacterName { get; set; } = string.Empty;

    /// <summary>
    /// Имя владельца персонажа на момент достижения.
    /// </summary>
    public string? UserFirstName { get; set; }

    /// <summary>
    /// Фамилия владельца персонажа на момент достижения.
    /// </summary>
    public string? UserLastName { get; set; }

    /// <summary>
    /// Название класса персонажа на момент достижения.
    /// </summary>
    public string? CharacterClassName { get; set; }

    /// <summary>
    /// Название специализации персонажа на момент достижения.
    /// </summary>
    public string? CharacterSpecName { get; set; }

    /// <summary>
    /// Идентификатор игровой сессии, по которой создана запись.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Тип номинации.
    /// </summary>
    public ChronicleNominationType NominationType { get; set; }

    /// <summary>
    /// Значение рекорда.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Момент, когда произошло достижение.
    /// </summary>
    public DateTime OccurredAtUtc { get; set; }

    /// <summary>
    /// Создаёт новую запись хроники.
    /// </summary>
    public static ChronicleRecord Create(
        Guid characterId,
        string characterName,
        string? userFirstName,
        string? userLastName,
        string? characterClassName,
        string? characterSpecName,
        Guid sessionId,
        ChronicleNominationType nominationType,
        decimal value,
        DateTime occurredAtUtc) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            CharacterId = characterId,
            CharacterName = characterName,
            UserFirstName = userFirstName,
            UserLastName = userLastName,
            CharacterClassName = characterClassName,
            CharacterSpecName = characterSpecName,
            SessionId = sessionId,
            NominationType = nominationType,
            Value = value,
            OccurredAtUtc = occurredAtUtc
        };

    /// <summary>
    /// Создаёт новую запись хроники.
    /// </summary>
    public static ChronicleRecord Create(
        Guid characterId,
        string characterName,
        Guid sessionId,
        ChronicleNominationType nominationType,
        decimal value,
        DateTime occurredAtUtc) =>
        Create(characterId, characterName, null, null, null, null, sessionId, nominationType, value, occurredAtUtc);
}
