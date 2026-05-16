namespace Chronicles.Application.Abstractions.Data;

/// <summary>
/// Участник завершённой игровой сессии.
/// </summary>
public sealed record CompletedGameSessionParticipantData(
    Guid UnitId,
    Guid? UserId,
    CompletedParticipantUnitType UnitType,
    string DisplayName,
    string? UserFirstName,
    string? UserLastName,
    string? CharacterClassName,
    string? CharacterSpecName);
