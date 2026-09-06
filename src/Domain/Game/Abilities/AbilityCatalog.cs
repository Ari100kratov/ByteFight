using Domain.Game.Statuses;
using Domain.ValueObjects;

namespace Domain.Game.Abilities;

/// <summary>
/// Каталог всех способностей игры. Баланс задаётся здесь и используется
/// боевым рантаймом и сидированием базы данных.
/// </summary>
public static class AbilityCatalog
{
    private static readonly Dictionary<AbilityType, AbilityDefinition> All = Build();

    /// <summary>
    /// Возвращает определение способности по типу.
    /// </summary>
    public static AbilityDefinition Get(AbilityType type) =>
        All.TryGetValue(type, out AbilityDefinition? definition)
            ? definition
            : throw new ArgumentOutOfRangeException(nameof(type), $"Способность {type} отсутствует в каталоге.");

    /// <summary>
    /// Пытается вернуть определение способности по типу.
    /// </summary>
    public static bool TryGet(AbilityType type, out AbilityDefinition definition) =>
        All.TryGetValue(type, out definition!);

    /// <summary>
    /// Все определения способностей.
    /// </summary>
    public static IReadOnlyCollection<AbilityDefinition> Definitions => All.Values;

    private static Dictionary<AbilityType, AbilityDefinition> Build()
    {
        var list = new List<AbilityDefinition>
        {
            // ===== Базовые атаки =====

            new AbilityDefinition(
                AbilityType.BasicMeleeAttack, "Верный удар", "Обычная атака ближнего боя.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 1, AbilityStatType.Damage, 18)),

            new AbilityDefinition(
                AbilityType.BasicRangedAttack, "Меткая стрела", "Обычная дальняя атака.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 4, AbilityStatType.Damage, 14)),

            new AbilityDefinition(
                AbilityType.Healing, "Светлый дар", "Базовое исцеление союзника.",
                AbilityEffectType.Healing, AbilityTargetType.Ally, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 3, AbilityStatType.Healing, 25)),

            // ===== Витязь =====

            new AbilityDefinition(
                AbilityType.VityazSwordStrike, "Рубящий удар", "Тяжёлый взмах мечом по одному врагу.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 1, AbilityStatType.Damage, 30,
                    AbilityStatType.Cooldown, 1, AbilityStatType.ManaCost, 8)),

            new AbilityDefinition(
                AbilityType.VityazShieldBash, "Удар щитом", "Оглушает врага на один ход.",
                AbilityEffectType.Debuff, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 1, AbilityStatType.Damage, 12,
                    AbilityStatType.Cooldown, 3, AbilityStatType.ManaCost, 12),
                [new AbilityStatusEffect(StatusEffectType.Stun, 1, 1)]),

            new AbilityDefinition(
                AbilityType.VityazCharge, "Богатырский рывок",
                "Бросок к врагу: урон и немощь. Дальность 4.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 4, AbilityStatType.Damage, 22,
                    AbilityStatType.Cooldown, 3, AbilityStatType.ManaCost, 15),
                [new AbilityStatusEffect(StatusEffectType.Weaken, 2, 15)],
                DashesToTarget: true),

            new AbilityDefinition(
                AbilityType.VityazBulwark, "Стойка богатыря",
                "Щит и мощь на три хода.",
                AbilityEffectType.Buff, AbilityTargetType.Self, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Cooldown, 4, AbilityStatType.ManaCost, 14),
                [
                    new AbilityStatusEffect(StatusEffectType.Shield, 3, 40),
                    new AbilityStatusEffect(StatusEffectType.Might, 3, 15)
                ]),

            new AbilityDefinition(
                AbilityType.VityazPerunWrath, "Гнев Перуна",
                "Клин молний перед собой: урон и горение.",
                AbilityEffectType.Damage, AbilityTargetType.Area, AbilityShape.Cone,
                Stats(AbilityStatType.Range, 3, AbilityStatType.AreaRadius, 2,
                    AbilityStatType.Damage, 34, AbilityStatType.Cooldown, 4, AbilityStatType.ManaCost, 22),
                [new AbilityStatusEffect(StatusEffectType.Burn, 2, 8)]),

            // ===== Волхв =====

            new AbilityDefinition(
                AbilityType.VolkhvPerunBolt, "Молния Перуна",
                "Линия разряда прошивает всё насквозь.",
                AbilityEffectType.Damage, AbilityTargetType.Area, AbilityShape.Line,
                Stats(AbilityStatType.Range, 5, AbilityStatType.Damage, 30,
                    AbilityStatType.Cooldown, 2, AbilityStatType.ManaCost, 18)),

            new AbilityDefinition(
                AbilityType.VolkhvFrostGrip, "Морозная длань",
                "Замедляет всех врагов в радиусе двух гексов.",
                AbilityEffectType.Debuff, AbilityTargetType.Area, AbilityShape.Radius,
                Stats(AbilityStatType.Range, 3, AbilityStatType.AreaRadius, 2,
                    AbilityStatType.Damage, 8, AbilityStatType.Cooldown, 3, AbilityStatType.ManaCost, 16),
                [new AbilityStatusEffect(StatusEffectType.Slow, 2, 1)]),

            new AbilityDefinition(
                AbilityType.VolkhvWard, "Оберег света",
                "Защитный щит союзнику или себе.",
                AbilityEffectType.Shield, AbilityTargetType.Ally, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 3, AbilityStatType.Cooldown, 3, AbilityStatType.ManaCost, 14),
                [new AbilityStatusEffect(StatusEffectType.Shield, 3, 45)]),

            new AbilityDefinition(
                AbilityType.VolkhvAshCloud, "Пепельный туман",
                "Облако золы: урон и немощь в радиусе.",
                AbilityEffectType.Damage, AbilityTargetType.Area, AbilityShape.Radius,
                Stats(AbilityStatType.Range, 4, AbilityStatType.AreaRadius, 1,
                    AbilityStatType.Damage, 20, AbilityStatType.Cooldown, 3, AbilityStatType.ManaCost, 18),
                [new AbilityStatusEffect(StatusEffectType.Weaken, 2, 20)]),

            new AbilityDefinition(
                AbilityType.VolkhvSvarogPillar, "Столб Сварожья",
                "Столб огня: сильный урон и горение в радиусе.",
                AbilityEffectType.Damage, AbilityTargetType.Area, AbilityShape.Radius,
                Stats(AbilityStatType.Range, 4, AbilityStatType.AreaRadius, 2,
                    AbilityStatType.Damage, 40, AbilityStatType.Cooldown, 5, AbilityStatType.ManaCost, 28),
                [new AbilityStatusEffect(StatusEffectType.Burn, 3, 12)]),

            // ===== Охотник =====

            new AbilityDefinition(
                AbilityType.HunterPreciseShot, "Меткий выстрел",
                "Выстрел в уязвимое место с любой дистанции.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 5, AbilityStatType.Damage, 32,
                    AbilityStatType.Cooldown, 1, AbilityStatType.ManaCost, 10)),

            new AbilityDefinition(
                AbilityType.HunterNetTrap, "Ловчая сеть",
                "Сковывает врага корнями на месте.",
                AbilityEffectType.Debuff, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 3, AbilityStatType.Cooldown, 3, AbilityStatType.ManaCost, 12),
                [new AbilityStatusEffect(StatusEffectType.Root, 2, 1)]),

            new AbilityDefinition(
                AbilityType.HunterPoisonArrow, "Ядовитая стрела",
                "Урон и долгий яд.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 4, AbilityStatType.Damage, 16,
                    AbilityStatType.Cooldown, 2, AbilityStatType.ManaCost, 14),
                [new AbilityStatusEffect(StatusEffectType.Poison, 3, 9)]),

            new AbilityDefinition(
                AbilityType.HunterCamouflage, "Маскировка",
                "Оберег среди чащи: меньше входящего урона.",
                AbilityEffectType.Buff, AbilityTargetType.Self, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Cooldown, 4, AbilityStatType.ManaCost, 10),
                [new AbilityStatusEffect(StatusEffectType.Ward, 3, 30)]),

            new AbilityDefinition(
                AbilityType.HunterVolley, "Град стрел",
                "Залп по области: урон и замедление.",
                AbilityEffectType.Damage, AbilityTargetType.Area, AbilityShape.Radius,
                Stats(AbilityStatType.Range, 5, AbilityStatType.AreaRadius, 2,
                    AbilityStatType.Damage, 26, AbilityStatType.Cooldown, 4, AbilityStatType.ManaCost, 22),
                [new AbilityStatusEffect(StatusEffectType.Slow, 2, 1)]),

            // ===== Ведунья =====

            new AbilityDefinition(
                AbilityType.VedunyaHerbalBrew, "Целебный отвар",
                "Сильное лечение союзника.",
                AbilityEffectType.Healing, AbilityTargetType.Ally, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 3, AbilityStatType.Healing, 45,
                    AbilityStatType.Cooldown, 1, AbilityStatType.ManaCost, 14)),

            new AbilityDefinition(
                AbilityType.VedunyaHex, "Порча",
                "Урон и немощь на цель.",
                AbilityEffectType.Debuff, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 4, AbilityStatType.Damage, 14,
                    AbilityStatType.Cooldown, 2, AbilityStatType.ManaCost, 12),
                [new AbilityStatusEffect(StatusEffectType.Weaken, 3, 25)]),

            new AbilityDefinition(
                AbilityType.VedunyaThorns, "Терновый круг",
                "Шипы из-под земли: урон и замедление в радиусе.",
                AbilityEffectType.Damage, AbilityTargetType.Area, AbilityShape.Radius,
                Stats(AbilityStatType.Range, 3, AbilityStatType.AreaRadius, 1,
                    AbilityStatType.Damage, 24, AbilityStatType.Cooldown, 3, AbilityStatType.ManaCost, 18),
                [new AbilityStatusEffect(StatusEffectType.Slow, 2, 1)]),

            new AbilityDefinition(
                AbilityType.VedunyaSpiritWard, "Дух-оберег",
                "Щит и регенерация союзнику.",
                AbilityEffectType.Shield, AbilityTargetType.Ally, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 3, AbilityStatType.Cooldown, 3, AbilityStatType.ManaCost, 16),
                [
                    new AbilityStatusEffect(StatusEffectType.Shield, 3, 35),
                    new AbilityStatusEffect(StatusEffectType.Regeneration, 3, 12)
                ]),

            new AbilityDefinition(
                AbilityType.VedunyaBearSpirit, "Медвежий дух",
                "Дух зверя: мощь и регенерация себе.",
                AbilityEffectType.Buff, AbilityTargetType.Self, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Cooldown, 4, AbilityStatType.ManaCost, 18),
                [
                    new AbilityStatusEffect(StatusEffectType.Might, 3, 25),
                    new AbilityStatusEffect(StatusEffectType.Regeneration, 3, 15)
                ]),

            // ===== Враги =====

            new AbilityDefinition(
                AbilityType.GhoulClawStrike, "Рваные когти", "Атака упыря когтями.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 1, AbilityStatType.Damage, 20)),

            new AbilityDefinition(
                AbilityType.GhoulFrenzy, "Поганое бешенство", "Упырь входит в ярость.",
                AbilityEffectType.Buff, AbilityTargetType.Self, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Cooldown, 4),
                [new AbilityStatusEffect(StatusEffectType.Might, 3, 25)]),

            new AbilityDefinition(
                AbilityType.LeshyRootVines, "Корни-оковы", "Леший сковывает жертву.",
                AbilityEffectType.Debuff, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 2, AbilityStatType.Cooldown, 3),
                [new AbilityStatusEffect(StatusEffectType.Root, 2, 1)]),

            new AbilityDefinition(
                AbilityType.LeshyRegeneration, "Сок земли", "Лес лечит лешего.",
                AbilityEffectType.Healing, AbilityTargetType.Self, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Healing, 30, AbilityStatType.Cooldown, 4)),

            new AbilityDefinition(
                AbilityType.KikimoraPoisonSpit, "Ядовитый плевок", "Кикимора плюётся ядом.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 3, AbilityStatType.Damage, 12),
                [new AbilityStatusEffect(StatusEffectType.Poison, 3, 7)]),

            new AbilityDefinition(
                AbilityType.WolfDashBite, "Прыжок с укусом", "Волколак прыгает и кусает.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 3, AbilityStatType.Damage, 24, AbilityStatType.Cooldown, 3),
                DashesToTarget: true),

            new AbilityDefinition(
                AbilityType.SkeletonBoneSlash, "Удар костяным мечом", "Скелет рубит ржавым клинком.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 1, AbilityStatType.Damage, 16)),

            new AbilityDefinition(
                AbilityType.BogCurseMiasma, "Миазмы порчи", "Болотный дух чадит порчей.",
                AbilityEffectType.Debuff, AbilityTargetType.Area, AbilityShape.Radius,
                Stats(AbilityStatType.Range, 3, AbilityStatType.AreaRadius, 1,
                    AbilityStatType.Damage, 10, AbilityStatType.Cooldown, 3),
                [new AbilityStatusEffect(StatusEffectType.Weaken, 2, 15)]),

            new AbilityDefinition(
                AbilityType.VampireDrain, "Высасывание жизни", "Упырь-князь пьёт кровь.",
                AbilityEffectType.Damage, AbilityTargetType.Enemy, AbilityShape.SingleTarget,
                Stats(AbilityStatType.Range, 1, AbilityStatType.Damage, 26, AbilityStatType.Cooldown, 2),
                [new AbilityStatusEffect(StatusEffectType.Regeneration, 2, 10)])
        };

        return list.ToDictionary(x => x.Type);
    }

    private static Dictionary<AbilityStatType, decimal> Stats(params object[] pairs)
    {
        var result = new Dictionary<AbilityStatType, decimal>();

        for (int i = 0; i + 1 < pairs.Length; i += 2)
        {
            result[(AbilityStatType)pairs[i]] = pairs[i + 1] switch
            {
                decimal exact => exact,
                int number => number,
                _ => throw new InvalidOperationException(
                    "Значение характеристики способности должно быть числом.")
            };
        }

        return result;
    }
}
