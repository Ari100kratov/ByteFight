using Domain.Game.Stats;

namespace Domain.Game.CharacterSpecs;

public sealed class CharacterSpecStat
{
    public Guid CharacterSpecId { get; set; }
    public StatType StatType { get; set; }
    public decimal Value { get; set; }

    public CharacterSpec CharacterSpec { get; set; }
}
