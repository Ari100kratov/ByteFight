using Domain.Game.CharacterClasses;
using Domain.GameRuntime.RuntimeLogEntries;
using Domain.ValueObjects;

namespace GameRuntime.World.Units;

internal sealed record PlayerUnit : BaseUnit
{
    public PlayerUnit(Position position, FacingDirection facingDirection)
        : base(position, facingDirection) { }

    public override Guid Id => CharacterId;

    public required Guid CharacterId { get; init; }

    public required CharacterClassType Class { get; init; }
}
