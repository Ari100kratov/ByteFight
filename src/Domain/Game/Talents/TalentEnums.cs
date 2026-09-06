using Domain.Game.Abilities;
using Domain.Game.Statuses;
using Domain.Game.Stats;

namespace Domain.Game.Talents;

/// <summary>
/// Тип узла таланта: пассивное свойство или открываемая активная способность.
/// </summary>
public enum TalentNodeType
{
    /// <summary>Пассивный бонус, действует постоянно.</summary>
    Passive = 1,

    /// <summary>Открывает активную способность.</summary>
    Active = 2
}

/// <summary>
/// Механика бонуса таланта.
/// </summary>
public enum TalentBonusType
{
    /// <summary>
    /// Прибавка к характеристике юнита за каждый ранг
    /// (например, здоровье, инициатива, мощь).
    /// </summary>
    StatBonus = 1,

    /// <summary>
    /// Снижение входящего урона в процентах за ранг.
    /// </summary>
    IncomingDamageReduction = 2,

    /// <summary>
    /// Прибавка к характеристике конкретной способности за ранг
    /// (урон, лечение, радиус и т.д.).
    /// </summary>
    AbilityStatBonus = 3,

    /// <summary>
    /// Сокращение перезарядки способности в ходах за ранг.
    /// </summary>
    AbilityCooldownReduction = 4,

    /// <summary>
    /// Открывает способность. Ранг всегда один.
    /// </summary>
    GrantsAbility = 5,

    /// <summary>
    /// Прибавка к силе статус-эффекта конкретной способности за ранг.
    /// </summary>
    AbilityStatusMagnitudeBonus = 6,

    /// <summary>
    /// Прибавка к длительности статус-эффектов способности за ранг.
    /// </summary>
    AbilityStatusDurationBonus = 7
}

/// <summary>
/// Описание одного бонуса, который даёт ранг таланта.
/// Заполненность полей зависит от <see cref="TalentBonusType"/>.
/// </summary>
public sealed record TalentBonus(
    TalentBonusType Type,
    StatType? Stat = null,
    AbilityType? Ability = null,
    AbilityStatType? AbilityStat = null,
    StatusEffectType? Status = null,
    decimal Value = 0);
