using Domain.GameRuntime.RuntimeLogEntries;
using Domain.ValueObjects;
using GameRuntime.Core.Units;

namespace GameRuntime.Logic.Actions;

public sealed class MoveAction : IRuntimeAction
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
