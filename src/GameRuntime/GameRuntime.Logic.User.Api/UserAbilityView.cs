using Domain.Game.Abilities;
using SharedKernel;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Представление способности юнита
/// </summary>
[UserCodeApi]
public sealed class UserAbilityView
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
    /// Приоритет способности.
    /// </summary>
    public required int Priority { get; init; }

    /// <summary>
    /// Характеристики способности.
    /// </summary>
    public required IReadOnlyDictionary<AbilityStatType, decimal> Stats { get; init; }

    /// <summary>
    /// Возвращает значение характеристики способности.
    /// </summary>
    public decimal? Get(AbilityStatType statType)
        => Stats.TryGetValue(statType, out decimal value)
            ? value
            : null;

    /// <summary>
    /// Проверяет, есть ли у способности указанная характеристика.
    /// </summary>
    public bool Has(AbilityStatType statType) => Stats.ContainsKey(statType);

    /// <summary>
    /// Дальность применения способности.
    /// </summary>
    public int Range => decimal.ToInt32(Math.Ceiling(Get(AbilityStatType.Range) ?? 0));

    /// <summary>
    /// Количество наносимого урона.
    /// </summary>
    public decimal Damage => Get(AbilityStatType.Damage) ?? 0;

    /// <summary>
    /// Количество восстанавливаемого здоровья.
    /// </summary>
    public decimal Healing => Get(AbilityStatType.Healing) ?? 0;

    /// <summary>
    /// Стоимость способности в мане.
    /// </summary>
    public decimal ManaCost => Get(AbilityStatType.ManaCost) ?? 0;

    /// <summary>
    /// Проверяет, достает ли способность до цели на указанной дистанции.
    /// </summary>
    public bool CanReach(int distance) => distance <= Range;
}
