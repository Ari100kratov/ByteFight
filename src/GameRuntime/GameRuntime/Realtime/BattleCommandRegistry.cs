using System.Collections.Concurrent;
using GameRuntime.Common.World;

namespace GameRuntime.Realtime;

/// <summary>
/// Реестр активных игровых сессий и их очередей команд.
/// Связывает SignalR-хаб с исполняющимися боями.
/// </summary>
public sealed class BattleCommandRegistry
{
    private readonly ConcurrentDictionary<Guid, BattleCommandQueue> sessions = new();

    /// <summary>
    /// Регистрирует новую сессию.
    /// </summary>
    public BattleCommandQueue Register(Guid sessionId)
    {
        var queue = new BattleCommandQueue();
        sessions[sessionId] = queue;

        return queue;
    }

    /// <summary>
    /// Отменяет регистрацию сессии и закрывает её очередь.
    /// </summary>
    public void Unregister(Guid sessionId)
    {
        if (sessions.TryRemove(sessionId, out BattleCommandQueue? queue))
        {
            queue.Complete();
        }
    }

    /// <summary>
    /// Пытается поставить команду в очередь активной сессии.
    /// Возвращает false, если сессия не найдена или очередь закрыта.
    /// </summary>
    public bool TryPost(Guid sessionId, PlayerBattleCommand command) =>
        sessions.TryGetValue(sessionId, out BattleCommandQueue? queue) &&
        queue.TryPost(command);
}
