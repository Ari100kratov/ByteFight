using Domain.Game.Stats;
using Domain.GameRuntime.RuntimeLogEntries;

namespace GameRuntime.World.Stats;

internal sealed class RuntimeStats
{
    public Dictionary<StatType, decimal> Current { get; }
    public Dictionary<StatType, decimal> Max { get; }

    public RuntimeStats(IEnumerable<(StatType Key, decimal Value)> baseStats)
    {
        Max = baseStats.ToDictionary(x => x.Key, x => x.Value);
        Current = Max.ToDictionary(k => k.Key, v => v.Value);
    }

    public decimal Get(StatType stat) => Current[stat];

    public StatSnapshot ApplyDamage(decimal value)
    {
        Current[StatType.Health] = Math.Max(0, Current[StatType.Health] - value);
        return new StatSnapshot(Current[StatType.Health], Max[StatType.Health]);
    }

    public void Heal(decimal value)
    {
        Current[StatType.Health] = Math.Min(Max[StatType.Health], Current[StatType.Health] + value);
    }

    public void Modify(StatType stat, decimal delta)
    {
        Max[stat] += delta;
        Current[stat] = Math.Min(Current[stat], Max[stat]);
    }
}
