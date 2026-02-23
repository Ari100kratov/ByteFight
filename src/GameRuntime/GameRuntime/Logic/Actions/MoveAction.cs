using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.World;
using GameRuntime.World.Units;

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

        yield return world.CreateWalkLogEntry(Actor.Id, Actor.FacingDirection, Actor.Position);
    }
}
