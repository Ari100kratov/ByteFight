using Domain.GameRuntime.GameSessions;

namespace Domain.GameRuntime.GameSessionParticipants;

public class GameSessionParticipant
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public ParticipantUnitType UnitType { get; set; }
    public UnitId UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public UserId? UserId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? CharacterClassName { get; set; }
    public string? CharacterSpecName { get; set; }
    public DateTime JoinedAt { get; set; }

    public GameSession Session { get; set; }
}
