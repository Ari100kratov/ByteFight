using Domain.Game.Stats;
using Domain.GameRuntime.RuntimeLogEntries;
using GameRuntime.World.Stats;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Представление характеристик юнита, доступное в пользовательском скрипте.
/// </summary>
public sealed class UserStatsView
{
    private readonly RuntimeStats _stats;

    internal UserStatsView(RuntimeStats stats)
    {
        _stats = stats;
    }

    /// <summary>
    /// Возвращает текущее значение характеристики.
    /// </summary>
    /// <param name="stat">Тип характеристики.</param>
    /// <returns>Текущее значение или <c>null</c>, если характеристика недоступна.</returns>
    public decimal? Get(StatType stat)
        => _stats.Current.TryGetValue(stat, out decimal value)
            ? value
            : null;

    /// <summary>
    /// Возвращает пару текущего и максимального значения характеристики.
    /// </summary>
    /// <param name="stat">Тип характеристики.</param>
    /// <returns>Снимок характеристики или <c>null</c>, если характеристика недоступна.</returns>
    public StatSnapshot? GetSnapshot(StatType stat)
        => _stats.Current.TryGetValue(stat, out decimal value)
            ? new StatSnapshot(value, _stats.Max[stat])
            : null;
}
