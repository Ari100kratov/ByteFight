using Domain.GameRuntime.RuntimeLogEntries;
using Domain.ValueObjects;
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

    public IEnumerable<RuntimeLogEntry> Execute()
    {
        Actor.Move(Position);

        yield return new WalkLogEntry(Actor.Id, Actor.FacingDirection, Actor.Position);
    }
}
