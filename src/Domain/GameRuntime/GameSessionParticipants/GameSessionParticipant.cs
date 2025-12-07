using Domain.GameRuntime.GameSessions;

namespace Domain.GameRuntime.GameSessionParticipants;

public class GameSessionParticipant
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public ParticipantUnitType UnitType { get; set; }
    public UnitId UnitId { get; set; }
    public UserId? UserId { get; set; }
    public DateTime JoinedAt { get; set; }

    public GameSession Session { get; set; }
}
