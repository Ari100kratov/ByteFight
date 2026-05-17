using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameSessions;
using IntegrationContracts.GameSessions;

namespace GameRuntime.Integration;

/// <summary>
/// Создаёт стабильное интеграционное событие завершения игровой сессии.
/// </summary>
public interface IGameSessionCompletedIntegrationEventFactory
{
    /// <summary>
    /// Формирует событие из завершённой доменной сессии и её журнала боя.
    /// </summary>
    GameSessionCompletedIntegrationEvent Create(GameSession session, IReadOnlyList<GameActionLogEntry> logs);
}
