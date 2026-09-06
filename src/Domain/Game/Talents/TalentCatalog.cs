using Domain.Game.Abilities;
using Domain.Game.CharacterClasses;
using Domain.Game.Stats;

namespace Domain.Game.Talents;

/// <summary>
/// Каталог деревьев талантов всех классов.
/// Контент задаётся кодом и версионируется вместе с приложением:
/// выбор игрока хранит только идентификаторы узлов и ранги.
/// </summary>
public static class TalentCatalog
{
    /// <summary>
    /// Возвращает дерево талантов класса.
    /// </summary>
    public static TalentTreeDefinition GetTree(CharacterClassType @class) => @class switch
    {
        CharacterClassType.Vityaz => Vityaz(),
        CharacterClassType.Volkhv => Volkhv(),
        CharacterClassType.Okhotnik => Okhotnik(),
        CharacterClassType.Vedunya => Vedunya(),
        _ => Legacy(@class)
    };

    /// <summary>
    /// Ищет узел таланта по идентификатору во всех деревьях.
    /// </summary>
    public static TalentNodeDefinition? FindNode(string talentId) => AllTrees()
        .SelectMany(tree => tree.Nodes)
        .FirstOrDefault(node => node.Id == talentId);

    public static IEnumerable<TalentTreeDefinition> AllTrees() =>
    [
        Vityaz(), Volkhv(), Okhotnik(), Vedunya()
    ];

    private static TalentTreeDefinition Legacy(CharacterClassType @class) => new(
        @class,
        [],
        []);

    // ===== Витязь =====

    private static TalentTreeDefinition Vityaz() => new(
        CharacterClassType.Vityaz,
        [
            new TalentBranchDefinition("sword", "Стезя Меча", "Ярость клинка и верный глаз ратника."),
            new TalentBranchDefinition("shield", "Стезя Щита", "Терпение камня и долг защитника.")
        ],
        [
            new TalentNodeDefinition(
                "vityaz_sword_steady_hand", "Твёрдая рука", "Исходящий урон повышен.",
                "sword", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Power, Value: 8)]),
            new TalentNodeDefinition(
                "vityaz_sword_tempered", "Богатырская закалка", "Максимум здоровья повышен.",
                "sword", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Health, Value: 35)]),
            new TalentNodeDefinition(
                "vityaz_sword_wild_swing", "Зверский замах", "Рубящий удар бьёт больнее.",
                "sword", 2, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatBonus,
                    Ability: AbilityType.VityazSwordStrike,
                    AbilityStat: AbilityStatType.Damage, Value: 12)]),
            new TalentNodeDefinition(
                "vityaz_sword_perun_wrath", "Гнев Перуна", "Открывает клин молний перед собой.",
                "sword", 3, 1, TalentNodeType.Active,
                [new TalentBonus(TalentBonusType.GrantsAbility, Ability: AbilityType.VityazPerunWrath)]),

            new TalentNodeDefinition(
                "vityaz_shield_oak_bark", "Древняя кора", "Броня повышена.",
                "shield", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Armor, Value: 3)]),
            new TalentNodeDefinition(
                "vityaz_shield_unshaken", "Непоколебимость", "Входящий урон снижен.",
                "shield", 2, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.IncomingDamageReduction, Value: 6)]),
            new TalentNodeDefinition(
                "vityaz_shield_bash_master", "Щитобой", "Удар щитом восстанавливается быстрее.",
                "shield", 2, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityCooldownReduction,
                    Ability: AbilityType.VityazShieldBash, Value: 1)]),
            new TalentNodeDefinition(
                "vityaz_shield_living_wall", "Живая стена", "Стойка богатыря даёт регенерацию.",
                "shield", 3, 1, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatusMagnitudeBonus,
                    Ability: AbilityType.VityazBulwark, Status: Domain.Game.Statuses.StatusEffectType.Regeneration, Value: 10)])
        ]);

    // ===== Волхв =====

    private static TalentTreeDefinition Volkhv() => new(
        CharacterClassType.Volkhv,
        [
            new TalentBranchDefinition("storm", "Стезя Грозы", "Перунова искра и громовое эхо."),
            new TalentBranchDefinition("ward", "Стезя Оберега", "Свет, хлад и защита рода.")
        ],
        [
            new TalentNodeDefinition(
                "volkhv_storm_spark", "Искровица", "Мощь стихий повышена.",
                "storm", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Power, Value: 8)]),
            new TalentNodeDefinition(
                "volkhv_storm_knowledge", "Ведание", "Запас маны и её восстановление выше.",
                "storm", 1, 2, TalentNodeType.Passive,
                [
                    new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Mana, Value: 20),
                    new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.ManaRegen, Value: 3)
                ]),
            new TalentNodeDefinition(
                "volkhv_storm_thunder_echo", "Громовое эхо", "Молния Перуна гремит сильнее.",
                "storm", 2, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatBonus,
                    Ability: AbilityType.VolkhvPerunBolt,
                    AbilityStat: AbilityStatType.Damage, Value: 15)]),
            new TalentNodeDefinition(
                "volkhv_storm_svarog", "Столб Сварожья", "Открывает столб очистительного огня.",
                "storm", 3, 1, TalentNodeType.Active,
                [new TalentBonus(TalentBonusType.GrantsAbility, Ability: AbilityType.VolkhvSvarogPillar)]),

            new TalentNodeDefinition(
                "volkhv_ward_herbal_knot", "Травяной узел", "Сила оберегов и лечения выше.",
                "ward", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Power, Value: 8)]),
            new TalentNodeDefinition(
                "volkhv_ward_snow_layer", "Снежный пласт", "Морозная длань захватывает шире.",
                "ward", 2, 1, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatBonus,
                    Ability: AbilityType.VolkhvFrostGrip,
                    AbilityStat: AbilityStatType.AreaRadius, Value: 1)]),
            new TalentNodeDefinition(
                "volkhv_ward_strong", "Крепкий оберег", "Оберег света держит дольше.",
                "ward", 2, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatusMagnitudeBonus,
                    Ability: AbilityType.VolkhvWard, Status: Domain.Game.Statuses.StatusEffectType.Shield, Value: 15)]),
            new TalentNodeDefinition(
                "volkhv_ward_cold_mind", "Хладный рассудок", "Инициатива и броня выше.",
                "ward", 3, 1, TalentNodeType.Passive,
                [
                    new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Initiative, Value: 3),
                    new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Armor, Value: 2)
                ])
        ]);

    // ===== Охотник =====

    private static TalentTreeDefinition Okhotnik() => new(
        CharacterClassType.Okhotnik,
        [
            new TalentBranchDefinition("bow", "Стезя Лука", "Дальний прицел и жгучие наконечники."),
            new TalentBranchDefinition("tracker", "Стезя Ловчего", "Тропы чащи и звериная смётка.")
        ],
        [
            new TalentNodeDefinition(
                "hunter_bow_sharp_sting", "Острое жало", "Урон выстрелов выше.",
                "bow", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Power, Value: 8)]),
            new TalentNodeDefinition(
                "hunter_bow_long_sight", "Дальний прицел", "Меткий выстрел летит дальше.",
                "bow", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatBonus,
                    Ability: AbilityType.HunterPreciseShot,
                    AbilityStat: AbilityStatType.Range, Value: 1)]),
            new TalentNodeDefinition(
                "hunter_bow_rotten_tips", "Гнилые наконечники", "Ядовитая стрела отравляет крепче.",
                "bow", 2, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatusMagnitudeBonus,
                    Ability: AbilityType.HunterPoisonArrow, Status: Domain.Game.Statuses.StatusEffectType.Poison, Value: 6)]),
            new TalentNodeDefinition(
                "hunter_bow_volley", "Град стрел", "Открывает залп по области.",
                "bow", 3, 1, TalentNodeType.Active,
                [new TalentBonus(TalentBonusType.GrantsAbility, Ability: AbilityType.HunterVolley)]),

            new TalentNodeDefinition(
                "hunter_tracker_paths", "Тропами чащ", "Очки перемещения выше.",
                "tracker", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.MoveRange, Value: 1)]),
            new TalentNodeDefinition(
                "hunter_tracker_snares", "Силки", "Ловчая сеть держит цель дольше.",
                "tracker", 2, 1, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatusDurationBonus,
                    Ability: AbilityType.HunterNetTrap, Value: 1)]),
            new TalentNodeDefinition(
                "hunter_tracker_camouflage", "Плащ чащобы", "Маскировка надёжнее.",
                "tracker", 2, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatusMagnitudeBonus,
                    Ability: AbilityType.HunterCamouflage, Status: Domain.Game.Statuses.StatusEffectType.Ward, Value: 10)]),
            new TalentNodeDefinition(
                "hunter_tracker_loyal_hound", "Верный ловчий", "Инициатива выше, входящий урон ниже.",
                "tracker", 3, 1, TalentNodeType.Passive,
                [
                    new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Initiative, Value: 3),
                    new TalentBonus(TalentBonusType.IncomingDamageReduction, Value: 6)
                ])
        ]);

    // ===== Ведунья =====

    private static TalentTreeDefinition Vedunya() => new(
        CharacterClassType.Vedunya,
        [
            new TalentBranchDefinition("herbs", "Стезя Трав", "Отвары, коренья и живительная сила."),
            new TalentBranchDefinition("whisper", "Стезя Шёпота", "Порча, колючки и слух земли.")
        ],
        [
            new TalentNodeDefinition(
                "vedunya_herbs_green_potion", "Зелёная аптека", "Сила лечения выше.",
                "herbs", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Power, Value: 8)]),
            new TalentNodeDefinition(
                "vedunya_herbs_thick_brew", "Густой отвар", "Целебный отвар лечит сильнее.",
                "herbs", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatBonus,
                    Ability: AbilityType.VedunyaHerbalBrew,
                    AbilityStat: AbilityStatType.Healing, Value: 15)]),
            new TalentNodeDefinition(
                "vedunya_herbs_deep_roots", "Глубокие корни", "Дух-оберег даёт больше регенерации.",
                "herbs", 2, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatusMagnitudeBonus,
                    Ability: AbilityType.VedunyaSpiritWard, Status: Domain.Game.Statuses.StatusEffectType.Regeneration, Value: 8)]),
            new TalentNodeDefinition(
                "vedunya_herbs_bear_spirit", "Медвежий дух", "Открывает призыв духа силы.",
                "herbs", 3, 1, TalentNodeType.Active,
                [new TalentBonus(TalentBonusType.GrantsAbility, Ability: AbilityType.VedunyaBearSpirit)]),

            new TalentNodeDefinition(
                "vedunya_whisper_sly", "Лукавый шёпот", "Порча кусает больнее.",
                "whisper", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Power, Value: 8)]),
            new TalentNodeDefinition(
                "vedunya_whisper_heavy_hex", "Тяжёлая порча", "Немощь от порчи сильнее.",
                "whisper", 1, 2, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatusMagnitudeBonus,
                    Ability: AbilityType.VedunyaHex, Status: Domain.Game.Statuses.StatusEffectType.Weaken, Value: 8)]),
            new TalentNodeDefinition(
                "vedunya_whisper_thorny_ring", "Колючий круг", "Терновый круг расползается шире.",
                "whisper", 2, 1, TalentNodeType.Passive,
                [new TalentBonus(TalentBonusType.AbilityStatBonus,
                    Ability: AbilityType.VedunyaThorns,
                    AbilityStat: AbilityStatType.AreaRadius, Value: 1)]),
            new TalentNodeDefinition(
                "vedunya_whisper_earth_ear", "Слух земли", "Инициатива и здоровье выше.",
                "whisper", 3, 1, TalentNodeType.Passive,
                [
                    new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Initiative, Value: 3),
                    new TalentBonus(TalentBonusType.StatBonus, Stat: StatType.Health, Value: 25)
                ])
        ]);
}
