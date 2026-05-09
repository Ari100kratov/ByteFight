using Domain.Game.Actions;
using Domain.ValueObjects;

namespace Domain.Game.CharacterSpecAbilities;

public sealed class CharacterSpecAbilityActionAsset
{
    public Guid CharacterSpecAbilityId { get; set; }

    public ActionType ActionType { get; set; }
    public int Variant { get; set; }
    public SpriteAnimation Animation { get; set; }

    public CharacterSpecAbility Ability { get; set; }
}
