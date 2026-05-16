using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions.Nominations;

/// <summary>
/// Изменение read-model одной номинации, полученное из завершённой игровой сессии.
/// </summary>
internal sealed record ChronicleNominationProjection(
    ChronicleNominationType NominationType,
    Guid CharacterId,
    string CharacterName,
    string? UserFirstName,
    string? UserLastName,
    string? CharacterClassName,
    string? CharacterSpecName,
    decimal Value,
    DateTime OccurredAtUtc,
    ChronicleNominationScoreUpdateMode ScoreUpdateMode,
    Guid SessionId,
    bool CreateRecord)
{
    /// <summary>
    /// Создаёт накопительное изменение результата номинации.
    /// </summary>
    public static ChronicleNominationProjection Increment(
        ChronicleNominationType nominationType,
        CompletedGameSessionParticipantData participant,
        decimal value,
        Guid sessionId,
        DateTime occurredAtUtc,
        bool createRecord = false) =>
        new(
            nominationType,
            participant.UnitId,
            participant.DisplayName,
            participant.UserFirstName,
            participant.UserLastName,
            participant.CharacterClassName,
            participant.CharacterSpecName,
            value,
            occurredAtUtc,
            ChronicleNominationScoreUpdateMode.Increment,
            sessionId,
            createRecord);

    /// <summary>
    /// Создаёт изменение, которое заменяет результат только при большем значении.
    /// </summary>
    public static ChronicleNominationProjection ReplaceIfGreater(
        ChronicleNominationType nominationType,
        CompletedGameSessionParticipantData participant,
        decimal value,
        Guid sessionId,
        DateTime occurredAtUtc,
        bool createRecord = true) =>
        new(
            nominationType,
            participant.UnitId,
            participant.DisplayName,
            participant.UserFirstName,
            participant.UserLastName,
            participant.CharacterClassName,
            participant.CharacterSpecName,
            value,
            occurredAtUtc,
            ChronicleNominationScoreUpdateMode.ReplaceIfGreater,
            sessionId,
            createRecord);

    /// <summary>
    /// Создаёт изменение, которое заменяет результат только при меньшем значении.
    /// </summary>
    public static ChronicleNominationProjection ReplaceIfLower(
        ChronicleNominationType nominationType,
        CompletedGameSessionParticipantData participant,
        decimal value,
        Guid sessionId,
        DateTime occurredAtUtc,
        bool createRecord = true) =>
        new(
            nominationType,
            participant.UnitId,
            participant.DisplayName,
            participant.UserFirstName,
            participant.UserLastName,
            participant.CharacterClassName,
            participant.CharacterSpecName,
            value,
            occurredAtUtc,
            ChronicleNominationScoreUpdateMode.ReplaceIfLower,
            sessionId,
            createRecord);
}
