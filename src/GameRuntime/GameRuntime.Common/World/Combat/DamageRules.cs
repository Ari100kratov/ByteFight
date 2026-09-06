using Domain.Game.Abilities;
using Domain.Game.Statuses;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Statuses;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Common.World.Combat;

/// <summary>
/// Расчёт исходящего урона и лечения способности с учётом мощи,
/// статусов и талантов применяющего.
/// </summary>
public static class DamageRules
{
    /// <summary>
    /// Рассчитывает итоговый урон способности: база, усиленная мощью
    /// применяющего и его статусами (мощь/немощь).
    /// </summary>
    public static decimal CalculateOutgoingDamage(BaseUnit actor, RuntimeAbility ability)
    {
        decimal baseDamage = ability.Get(AbilityStatType.Damage);

        if (baseDamage <= 0)
        {
            return 0;
        }

        decimal percentBonus = actor.Stats.GetPower() + actor.BattleState.Statuses.OutgoingDamageBonus;

        return decimal.Round(baseDamage * (1 + percentBonus / 100), 1);
    }

    /// <summary>
    /// Рассчитывает итоговое лечение способности.
    /// </summary>
    public static decimal CalculateOutgoingHealing(BaseUnit actor, RuntimeAbility ability)
    {
        decimal baseHealing = ability.Get(AbilityStatType.Healing);

        if (baseHealing <= 0)
        {
            return 0;
        }

        decimal percentBonus = actor.Stats.GetPower();

        return decimal.Round(baseHealing * (1 + percentBonus / 100), 1);
    }

    /// <summary>
    /// Применяет входящий урон к цели: броня, оберег, рельеф и щит.
    /// Возвращает фактически снятое здоровье.
    /// </summary>
    public static decimal ApplyIncomingDamage(ArenaWorld world, BaseUnit target, decimal rawDamage)
    {
        if (rawDamage <= 0)
        {
            return 0;
        }

        decimal damage = rawDamage - target.Stats.GetArmor();

        // Оберег: процентное снижение входящего урона.
        decimal ward = target.BattleState.Statuses.IncomingDamageReduction;

        if (ward > 0)
        {
            damage *= 1 - Math.Min(90, ward) / 100;
        }

        // Рельеф под целью: чащоба укрывает, топь открывает.
        damage *= target.GetTerrainDamageMultiplier(world.Arena.Terrain);

        damage = decimal.Round(Math.Max(0, damage), 1);

        // Щит поглощает урон, пока пул не иссякнет.
        damage = AbsorbByShield(target, damage);

        if (damage <= 0)
        {
            return 0;
        }

        target.Stats.ApplyDamage(damage);

        return damage;
    }

    /// <summary>
    /// Списывает урон из пула щита и возвращает остаток урона.
    /// </summary>
    public static decimal AbsorbByShield(BaseUnit target, decimal damage)
    {
        RuntimeStatusEffect? shield = target.BattleState.Statuses.Get(StatusEffectType.Shield);

        if (shield is null || shield.Magnitude <= 0)
        {
            return damage;
        }

        decimal absorbed = Math.Min(shield.Magnitude, damage);
        decimal remaining = damage - absorbed;

        target.BattleState.Statuses.Set(
            StatusEffectType.Shield,
            shield.RemainingTurns,
            shield.Magnitude - absorbed);

        return remaining;
    }
}
