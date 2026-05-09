using Domain.Game.Abilities;

namespace GameRuntime.Common.World.Abilities;

/// <summary>
/// Описывает способность юнита, доступную во время боя.
/// </summary>
public sealed class RuntimeAbility
{
    /// <summary>
    /// Тип способности.
    /// </summary>
    public required AbilityType Type { get; init; }

    /// <summary>
    /// Тип эффекта способности.
    /// </summary>
    public required AbilityEffectType EffectType { get; init; }

    /// <summary>
    /// Тип цели способности.
    /// </summary>
    public required AbilityTargetType TargetType { get; init; }

    /// <summary>
    /// Название способности.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Приоритет выбора способности.
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// Характеристики способности.
    /// </summary>
    public IReadOnlyDictionary<AbilityStatType, decimal> Stats { get; init; } =
        new Dictionary<AbilityStatType, decimal>();

    /// <summary>
    /// Возвращает значение характеристики способности.
    /// </summary>
    public decimal Get(AbilityStatType statType) =>
        Stats.GetValueOrDefault(statType);

    /// <summary>
    /// Проверяет, есть ли у способности указанная характеристика.
    /// </summary>
    public bool Has(AbilityStatType statType) =>
        Stats.ContainsKey(statType);

    /// <summary>
    /// Возвращает дальность применения способности.
    /// </summary>
    public int GetRange() =>
        decimal.ToInt32(Math.Ceiling(Get(AbilityStatType.Range)));

    /// <summary>
    /// Проверяет, может ли способность быть применена на указанной дистанции.
    /// </summary>
    public bool CanReach(int distance) =>
        distance <= GetRange();
}
