using Domain.Game.Actions;
using Domain.ValueObjects;

namespace Domain.Game.Enemies;

public sealed class EnemyAbilityActionAsset
{
    public Guid EnemyAbilityId { get; set; }

    public ActionType ActionType { get; set; }
    public int Variant { get; set; }
    public SpriteAnimation Animation { get; set; }

    public EnemyAbility Ability { get; set; }
}
