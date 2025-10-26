using Domain.Game.Stats;

namespace Domain.Game.CharacterClasses;

public sealed class CharacterClassStat
{
    public Guid CharacterClassId { get; set; }
    public StatType StatType { get; set; }
    public decimal Value { get; set; }

    public CharacterClass CharacterClass { get; set; }
}
