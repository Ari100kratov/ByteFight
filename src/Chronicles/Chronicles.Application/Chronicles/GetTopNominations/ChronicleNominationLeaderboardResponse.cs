namespace Chronicles.Application.Chronicles.GetTopNominations;

/// <summary>
/// Лидерборд по одной номинации.
/// </summary>
public sealed record ChronicleNominationLeaderboardResponse(
    string Code,
    string Title,
    string Description,
    string MetricLabel,
    string MetricUnit,
    string ValueKind,
    string SortDirection,
    IReadOnlyList<ChronicleNominationEntryResponse> Entries);

/// <summary>
/// Одна строка лидерборда номинации.
/// </summary>
public sealed record ChronicleNominationEntryResponse(
    int Rank,
    Guid CharacterId,
    string CharacterName,
    string? UserFirstName,
    string? UserLastName,
    string? CharacterClassName,
    string? CharacterSpecName,
    decimal Value,
    Guid? SessionId,
    DateTime? OccurredAtUtc);
