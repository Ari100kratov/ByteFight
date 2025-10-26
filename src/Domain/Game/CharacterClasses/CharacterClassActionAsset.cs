using Domain.Game.Actions;
using Domain.ValueObjects;

namespace Domain.Game.CharacterClasses;

public sealed class CharacterClassActionAsset
{
    public Guid CharacterClassId { get; set; }
    public ActionType ActionType { get; set; }
    public int Variant { get; set; }
    public SpriteAnimation Animation { get; set; }

    public CharacterClass CharacterClass { get; set; }
}
