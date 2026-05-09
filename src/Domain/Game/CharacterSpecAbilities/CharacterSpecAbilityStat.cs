using Domain.Game.Abilities;

namespace Domain.Game.CharacterSpecAbilities;

public sealed class CharacterSpecAbilityStat
{
    public Guid CharacterSpecAbilityId { get; set; }
    public AbilityStatType StatType { get; set; }
    public decimal Value { get; set; }

    public CharacterSpecAbility Ability { get; set; }
}
