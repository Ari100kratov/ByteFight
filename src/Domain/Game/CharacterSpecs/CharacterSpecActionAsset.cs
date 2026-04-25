using Domain.Game.Actions;
using Domain.ValueObjects;

namespace Domain.Game.CharacterSpecs;

public sealed class CharacterSpecActionAsset
{
    public Guid CharacterSpecId { get; set; }
    public ActionType ActionType { get; set; }
    public int Variant { get; set; }
    public SpriteAnimation Animation { get; set; }

    public CharacterSpec CharacterSpec { get; set; }
}
