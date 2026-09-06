using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.ValueObjects;
using GameRuntime.Common;
using GameRuntime.Common.World;
using GameRuntime.Common.World.ArenaItems;
using GameRuntime.Common.World.Stats;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Logic.Actions;

/// <summary>
/// Перемещение юнита по пути из соседних гексов в пределах
/// оставшихся очков перемещения.
/// </summary>
internal sealed class MoveAction : IRuntimeAction
{
    public BaseUnit Actor { get; }

    public IReadOnlyList<Position> Path { get; }

    public MoveAction(BaseUnit actor, IReadOnlyList<Position> path)
    {
        Actor = actor;
        Path = path;
    }

    public IEnumerable<GameActionLogEntry> Execute(ArenaWorld world)
    {
        if (!MovementRules.IsPathValid(world, Actor, Path, out int totalCost))
        {
            yield return world.CreateIdleLogEntry(Actor, IdleReasons.MoveImpossible);
            yield break;
        }

        foreach (Position step in Path.Skip(1))
        {
            Actor.MoveStep(step);
        }

        Actor.BattleState.MovePointsRemaining -= totalCost;

        yield return world.CreateWalkLogEntry(Actor, Path);

        if (Actor is not PlayerUnit)
        {
            yield break;
        }

        ArenaItemDefinition? item = world.Arena.GetItemAt(Actor.Position);
        if (item is null)
        {
            yield break;
        }

        ArenaItemDefinition pickedItem = world.Arena.RemoveItem(item.PlacedItemId);

        StatApplyResult effect = Actor.ApplyItem(pickedItem);

        yield return world.CreateItemPickedUpLogEntry(
            Actor,
            pickedItem,
            effect.AppliedValue,
            effect.Snapshot);
    }
}
