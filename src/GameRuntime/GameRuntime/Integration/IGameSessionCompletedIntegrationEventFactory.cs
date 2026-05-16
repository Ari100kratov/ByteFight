using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameSessions;
using IntegrationContracts.GameSessions;

namespace GameRuntime.Integration;

public interface IGameSessionCompletedIntegrationEventFactory
{
    GameSessionCompletedIntegrationEvent Create(GameSession session, IReadOnlyList<GameActionLogEntry> logs);
}
