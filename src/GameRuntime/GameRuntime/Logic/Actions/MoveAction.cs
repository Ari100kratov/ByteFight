using Domain.Game.Stats;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.ValueObjects;
using GameRuntime.Common.World;
using GameRuntime.Common.World.ArenaItems;
using GameRuntime.Common.World.Stats;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Logic.Actions;

internal sealed class MoveAction : IRuntimeAction
{
    public BaseUnit Actor { get; }

    public Position Position { get; }

    public MoveAction(BaseUnit actor, Position position)
    {
        Actor = actor;
        Position = position;
    }

    public IEnumerable<GameActionLogEntry> Execute(ArenaWorld world)
    {
        Actor.Move(Position);

        yield return world.CreateWalkLogEntry(Actor);

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
