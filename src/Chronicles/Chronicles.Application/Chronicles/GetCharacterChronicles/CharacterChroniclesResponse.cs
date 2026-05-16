namespace Chronicles.Application.Chronicles.GetCharacterChronicles;

/// <summary>
/// Подробная хроника персонажа.
/// </summary>
public sealed record CharacterChroniclesResponse(
    Guid CharacterId,
    string CharacterName,
    int BattlesPlayed,
    int Victories,
    int Defeats,
    int Draws,
    int TotalTurnsPlayed,
    DateTime? LastSessionAtUtc,
    IReadOnlyList<CharacterChronicleRecordResponse> RecentRecords);

/// <summary>
/// Краткая запись о недавнем достижении персонажа.
/// </summary>
public sealed record CharacterChronicleRecordResponse(
    string NominationCode,
    string? UserFirstName,
    string? UserLastName,
    string? CharacterClassName,
    string? CharacterSpecName,
    decimal Value,
    DateTime OccurredAtUtc);
