using Domain.GameRuntime.GameActionLogs.Entries;
using GameRuntime.Common.World;

namespace GameRuntime.Logic.Actions;

internal interface IRuntimeAction
{
    IEnumerable<GameActionLogEntry> Execute(ArenaWorld world);
}
