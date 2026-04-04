using Domain.Game.Stats;
using Domain.ValueObjects;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Представление характеристик юнита.
/// </summary>
public sealed class UserStatsView
{
    public required IReadOnlyDictionary<StatType, decimal> Current { get; init; }
    public required IReadOnlyDictionary<StatType, decimal> Max { get; init; }

    /// <summary>
    /// Возвращает текущее значение характеристики.
    /// </summary>
    /// <param name="statType">Тип характеристики.</param>
    /// <returns>Текущее значение или <c>null</c>, если характеристика недоступна.</returns>
    public decimal? Get(StatType statType)
        => Current.TryGetValue(statType, out decimal value)
            ? value
            : null;

    /// <summary>
    /// Возвращает снапшот текущего и максимального значения характеристики.
    /// </summary>
    /// <param name="stat">Тип характеристики.</param>
    /// <returns>Снапшот характеристики или <c>null</c>, если характеристика отсутствует.</returns>
    public StatSnapshot? GetSnapshot(StatType stat)
        => Current.TryGetValue(stat, out decimal value) && Max.TryGetValue(stat, out decimal max)
            ? new StatSnapshot(value, max)
            : null;
}
