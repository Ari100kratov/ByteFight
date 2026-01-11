using Domain.Game.Stats;
using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.World.Stats;

namespace GameRuntime.Logic.User.Api;

public sealed class UserStatsView
{
    private readonly RuntimeStats _stats;

    internal UserStatsView(RuntimeStats stats)
    {
        _stats = stats;
    }

    public decimal? Get(StatType stat)
        => _stats.Current.TryGetValue(stat, out decimal value)
            ? value
            : null;

    public StatSnapshot? GetSnapshot(StatType stat)
        => _stats.Current.TryGetValue(stat, out decimal value)
            ? new StatSnapshot(value, _stats.Max[stat])
            : null;
}
