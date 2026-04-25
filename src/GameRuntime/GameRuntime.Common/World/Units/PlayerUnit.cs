using Domain.Game.CharacterSpecs;
using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;

namespace GameRuntime.Common.World.Units;

public sealed record PlayerUnit : BaseUnit
{
    public PlayerUnit(Position position, FacingDirection facingDirection)
        : base(position, facingDirection) { }

    public override Guid Id => CharacterId;

    public required Guid CharacterId { get; init; }

    public required CharacterSpecType Spec { get; init; }
}
