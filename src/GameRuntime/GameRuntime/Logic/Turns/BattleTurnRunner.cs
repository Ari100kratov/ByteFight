using Domain.GameRuntime.GameActionLogs.Entries;
using GameRuntime.Common.World;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Logic.Turns;

/// <summary>
/// Исполняет ход одного юнита. Реализации: ИИ врагов,
/// ручное управление игроком и исполнение пользовательского кода.
/// </summary>
internal interface IBattleTurnRunner
{
    Task<IReadOnlyList<GameActionLogEntry>> PlayTurn(
        BaseUnit unit,
        ArenaWorld world,
        Action<IReadOnlyList<GameActionLogEntry>> onActionPerformed,
        CancellationToken ct);
}
