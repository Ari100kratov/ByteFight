using Domain.GameRuntime.GameActionLogs.Entries;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Logic.Turns;

/// <summary>
/// Пошаговый движок боя: ведёт раунды по инициативе, раздаёт ходы
/// раннерам (ИИ, ручное управление, пользовательский код)
/// и определяет завершение боя.
/// </summary>
internal sealed class BattleEngine
{
    private readonly ArenaWorld world;
    private readonly IBattleTurnRunner npcRunner;
    private readonly IBattleTurnRunner playerRunner;
    private readonly Action<IReadOnlyList<GameActionLogEntry>>? broadcastAction;

    private bool roundStarted;

    /// <summary>
    /// Создаёт движок боя.
    /// </summary>
    /// <param name="world">Мир арены.</param>
    /// <param name="npcRunner">Раннер ходов врагов.</param>
    /// <param name="playerRunner">Раннер хода игрока (ручной или скриптовый).</param>
    /// <param name="broadcastAction">Колбэк немедленной рассылки логов действия.</param>
    public BattleEngine(
        ArenaWorld world,
        IBattleTurnRunner npcRunner,
        IBattleTurnRunner playerRunner,
        Action<IReadOnlyList<GameActionLogEntry>>? broadcastAction)
    {
        this.world = world;
        this.npcRunner = npcRunner;
        this.playerRunner = playerRunner;
        this.broadcastAction = broadcastAction;
    }

    /// <summary>
    /// Выполняет следующий атомарный шаг боя: начало раунда, ход юнита
    /// (целиком) или ничего, если бой уже завершён.
    /// </summary>
    public async Task<BattleStepResult> StepAsync(CancellationToken ct)
    {
        if (world.CheckGameOver() is not null)
        {
            return new BattleStepResult([], true, false);
        }

        var logs = new List<GameActionLogEntry>();

        // Начинаем раунд, если очередь пуста.
        if (!roundStarted || world.RoundOrder.Count == 0)
        {
            logs.Add(world.StartNextRound());
            roundStarted = true;

            return new BattleStepResult(logs, false, false);
        }

        BaseUnit? unit = world.AdvanceToNextUnit();

        if (unit is null)
        {
            // Раунд исчерпан — следующий шаг начнёт новый.
            roundStarted = false;
            return new BattleStepResult(logs, false, false);
        }

        logs.AddRange(world.BeginUnitTurn(unit));

        broadcastAction?.Invoke(logs);

        // Юнит мог погибнуть от периодического урона или оглушён.
        if (unit.IsDead || unit.BattleState.ActionsRemaining <= 0 && unit.BattleState.MovePointsRemaining <= 0)
        {
            return new BattleStepResult(logs, false, world.CheckGameOver() is not null);
        }

        IBattleTurnRunner runner = unit is PlayerUnit ? playerRunner : npcRunner;

        IReadOnlyList<GameActionLogEntry> turnLogs =
            await runner.PlayTurn(unit, world, broadcastAction ?? NoBroadcast, ct);

        logs.AddRange(turnLogs);

        // Длительности статусов уменьшаются в конце хода носителя.
        world.EndUnitTurn(unit);

        return new BattleStepResult(logs, world.CheckGameOver() is not null, false);
    }

    private static void NoBroadcast(IReadOnlyList<GameActionLogEntry> _)
    {
        // Намеренно пусто: рассылка не требуется, когда колбэк не задан.
    }
}

/// <summary>
/// Результат шага боя.
/// </summary>
/// <param name="Logs">Записи журнала, порождённые шагом.</param>
/// <param name="Finished">Бой завершён.</param>
/// <param name="EmptyStep">Шаг не произвёл записей (например, между раундами).</param>
internal sealed record BattleStepResult(
    IReadOnlyList<GameActionLogEntry> Logs,
    bool Finished,
    bool EmptyStep);
