using Domain.GameRuntime.GameActionLogs;
using GameRuntime.World;

namespace GameRuntime.Logic.Actions;

internal interface IRuntimeAction
{
    IEnumerable<GameActionLogEntry> Execute(ArenaWorld world);
}
