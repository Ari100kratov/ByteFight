using Domain.Game.Abilities;
using Domain.Game.Statuses;
using Domain.Game.Talents;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Builders;

/// <summary>
/// Применяет выбранные таланты персонажа к его боевому аватару:
/// прибавки к характеристикам, улучшения способностей
/// и открывание новых способностей.
/// </summary>
internal static class TalentEffectsApplier
{
    /// <summary>
    /// Применяет таланты к юниту игрока на старте боя.
    /// </summary>
    public static void Apply(PlayerUnit unit, IReadOnlyDictionary<string, int> chosenTalents)
    {
        if (chosenTalents.Count == 0)
        {
            return;
        }

        foreach ((string talentId, int rank) in chosenTalents)
        {
            TalentNodeDefinition? node = TalentCatalog.FindNode(talentId);

            if (node is null)
            {
                continue;
            }

            foreach (TalentBonus bonus in node.Bonuses)
            {
                ApplyBonus(unit, bonus, rank);
            }
        }
    }

    private static void ApplyBonus(PlayerUnit unit, TalentBonus bonus, int rank)
    {
        switch (bonus.Type)
        {
            case TalentBonusType.StatBonus:
                if (bonus.Stat.HasValue)
                {
                    unit.Stats.Modify(bonus.Stat.Value, bonus.Value * rank);
                }

                break;

            case TalentBonusType.IncomingDamageReduction:
                unit.BattleState.Statuses.Apply(
                    StatusEffectType.Ward,
                    int.MaxValue,
                    bonus.Value * rank);

                break;

            case TalentBonusType.GrantsAbility:
                if (bonus.Ability.HasValue && !unit.Abilities.Has(bonus.Ability.Value))
                {
                    RuntimeAbility granted = RuntimeAbilityFactory.Create(
                        bonus.Ability.Value,
                        priority: 5);

                    unit.AddAbility(granted);
                }

                break;

            case TalentBonusType.AbilityStatBonus:
                if (bonus.Ability.HasValue &&
                    bonus.AbilityStat.HasValue &&
                    unit.Abilities.TryGet(bonus.Ability.Value, out RuntimeAbility? upgraded))
                {
                    upgraded.AddStat(bonus.AbilityStat.Value, bonus.Value * rank);
                }

                break;

            case TalentBonusType.AbilityCooldownReduction:
                if (bonus.Ability.HasValue &&
                    unit.Abilities.TryGet(bonus.Ability.Value, out RuntimeAbility? cooled))
                {
                    cooled.AddStat(AbilityStatType.Cooldown, -bonus.Value * rank);
                }

                break;

            case TalentBonusType.AbilityStatusMagnitudeBonus:
                if (bonus.Ability.HasValue &&
                    bonus.Status.HasValue &&
                    unit.Abilities.TryGet(bonus.Ability.Value, out RuntimeAbility? magnified))
                {
                    magnified.BoostStatusMagnitude(
                        bonus.Status.Value,
                        bonus.Value * rank);
                }

                break;

            case TalentBonusType.AbilityStatusDurationBonus:
                if (bonus.Ability.HasValue &&
                    unit.Abilities.TryGet(bonus.Ability.Value, out RuntimeAbility? extended))
                {
                    extended.BoostStatusDuration((int)bonus.Value * rank);
                }

                break;
        }
    }
}
