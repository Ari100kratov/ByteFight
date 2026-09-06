using Domain.Game.CharacterSpecs;
using Domain.GameRuntime.GameActionLogs;
using Domain.ValueObjects;
using GameRuntime.Common.World.Abilities;

namespace GameRuntime.Common.World.Units;

public sealed record PlayerUnit : BaseUnit
{
    public PlayerUnit(Position position, FacingDirection facingDirection)
        : base(position, facingDirection) { }

    public override Guid Id => CharacterId;

    public required Guid CharacterId { get; init; }

    public required CharacterSpecType Spec { get; init; }

    /// <summary>
    /// Уровень персонажа владельца (влияет на отображение).
    /// </summary>
    public int Level { get; init; } = 1;

    /// <summary>
    /// Добавляет способность персонажу (открывается талантом).
    /// </summary>
    public void AddAbility(RuntimeAbility ability) => Abilities.Add(ability);
}
