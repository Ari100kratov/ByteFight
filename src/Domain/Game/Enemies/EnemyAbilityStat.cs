using Domain.Game.Abilities;

namespace Domain.Game.Enemies;

public sealed class EnemyAbilityStat
{
    public Guid EnemyAbilityId { get; set; }
    public AbilityStatType StatType { get; set; }
    public decimal Value { get; set; }

    public EnemyAbility Ability { get; set; }
}
