using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

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
