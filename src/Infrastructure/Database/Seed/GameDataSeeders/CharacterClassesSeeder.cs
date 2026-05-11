using Application.Abstractions.Data;
using Domain.Game.Abilities;
using Domain.Game.Actions;
using Domain.Game.CharacterClasses;
using Domain.Game.CharacterSpecAbilities;
using Domain.Game.CharacterSpecs;
using Domain.Game.Stats;
using Domain.ValueObjects;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal static class CharacterClassesSeeder
{
    private const string AssetRoot = "classes";

    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        CharacterClass warrior = CreateWarrior(seed);
        CharacterClass mage = CreateMage(seed);

        dbContext.CharacterClasses.AddRange(warrior, mage);

        seed.Class_Warrior = warrior.Id;
        seed.Class_Mage = mage.Id;
    }

    private static CharacterClass CreateWarrior(SeedContext seed)
    {
        CharacterClass warrior = new()
        {
            Id = Guid.CreateVersion7(),
            Type = CharacterClassType.Warrior,
            Name = "Воин",
            Description = "Сильный боец ближнего боя, полагающийся на выносливость, оружие и позицию."
        };

        CharacterSpec berserker = CreateWarriorSpec(
            warrior,
            CharacterSpecType.Berserker,
            "Берсерк",
            "Агрессивный воин, жертвующий защитой ради высокой силы удара.",
            folder: "berserker",
            health: 170,
            meleeDamage: 40,
            moveRange: 2,
            specIdSetter: id => seed.Spec_Warrior_Berserker = id);

        CharacterSpec guardian = CreateWarriorSpec(
            warrior,
            CharacterSpecType.Guardian,
            "Страж",
            "Выносливый защитник, способный долго держать линию фронта.",
            folder: "guardian",
            health: 260,
            meleeDamage: 30,
            moveRange: 2,
            specIdSetter: id => seed.Spec_Warrior_Guardian = id);

        CharacterSpec duelist = CreateWarriorSpec(
            warrior,
            CharacterSpecType.Duelist,
            "Дуэлянт",
            "Подвижный мастер ближнего боя, побеждающий за счёт темпа и точности.",
            folder: "duelist",
            health: 185,
            meleeDamage: 36,
            moveRange: 3,
            specIdSetter: id => seed.Spec_Warrior_Duelist = id);

        warrior.Specs = [berserker, guardian, duelist];

        return warrior;
    }

    private static CharacterClass CreateMage(SeedContext seed)
    {
        CharacterClass mage = new()
        {
            Id = Guid.CreateVersion7(),
            Type = CharacterClassType.Mage,
            Name = "Маг",
            Description = "Заклинатель дальнего боя, управляющий магической энергией и наносящий урон с расстояния."
        };

        CharacterSpec pyromancer = CreateMageSpec(
            mage,
            CharacterSpecType.Pyromancer,
            "Пиромант",
            "Боевой маг огня, наносящий высокий урон разрушительными заклинаниями.",
            folder: "pyromancer",
            health: 120,
            meleeDamage: 20,
            rangedDamage: 40,
            rangedRange: 4,
            moveRange: 2,
            mana: 100,
            specIdSetter: id => seed.Spec_Mage_Pyromancer = id);

        CharacterSpec luminary = CreateMageSpec(
            mage,
            CharacterSpecType.Luminary,
            "Люминар",
            "Маг света, использующий концентрированную энергию для точечных атак на расстоянии.",
            folder: "luminary",
            health: 135,
            meleeDamage: 18,
            rangedDamage: 36,
            rangedRange: 3,
            moveRange: 2,
            mana: 115,
            specIdSetter: id => seed.Spec_Mage_Luminary = id);

        CharacterSpec arcanist = CreateMageSpec(
            mage,
            CharacterSpecType.Arcanist,
            "Арканист",
            "Универсальный маг тайной школы, способный атаковать одной базовой магической атакой даже вблизи.",
            folder: "arcanist",
            health: 150,
            meleeDamage: null,
            rangedDamage: 32,
            rangedRange: 2,
            moveRange: 2,
            mana: 130,
            specIdSetter: id => seed.Spec_Mage_Arcanist = id);

        mage.Specs = [pyromancer, luminary, arcanist];

        return mage;
    }

    private static CharacterSpec CreateWarriorSpec(
        CharacterClass characterClass,
        CharacterSpecType type,
        string name,
        string description,
        string folder,
        int health,
        int meleeDamage,
        int moveRange,
        Action<Guid> specIdSetter)
    {
        string specFolder = GetSpecFolder(CharacterClassType.Warrior, folder);

        CharacterSpec spec = new()
        {
            Id = Guid.CreateVersion7(),
            ClassId = characterClass.Id,
            Type = type,
            Name = name,
            PortraitUrl = $"{specFolder}/Portrait.png",
            Description = description,
            Stats =
            [
                CreateStat(StatType.Health, health),
                CreateStat(StatType.MoveRange, moveRange)
            ],
            ActionAssets = CreateWarriorActionAssets(type, specFolder),
            Abilities = CreateWarriorAbilities(type, specFolder, meleeDamage)
        };

        specIdSetter(spec.Id);

        return spec;
    }

    private static CharacterSpec CreateMageSpec(
        CharacterClass characterClass,
        CharacterSpecType type,
        string name,
        string description,
        string folder,
        int health,
        int? meleeDamage,
        int rangedDamage,
        int rangedRange,
        int moveRange,
        int mana,
        Action<Guid> specIdSetter)
    {
        string specFolder = GetSpecFolder(CharacterClassType.Mage, folder);

        CharacterSpec spec = new()
        {
            Id = Guid.CreateVersion7(),
            ClassId = characterClass.Id,
            Type = type,
            Name = name,
            PortraitUrl = $"{specFolder}/Portrait.png",
            Description = description,
            Stats =
            [
                CreateStat(StatType.Health, health),
                CreateStat(StatType.MoveRange, moveRange),
                CreateStat(StatType.Mana, mana)
            ],
            ActionAssets = CreateMageActionAssets(type, specFolder),
            Abilities = CreateMageAbilities(type, specFolder, meleeDamage, rangedDamage, rangedRange)
        };

        specIdSetter(spec.Id);

        return spec;
    }

    private static CharacterSpecStat CreateStat(StatType type, int value) =>
        new()
        {
            StatType = type,
            Value = value
        };

    private static CharacterSpecAbility CreateBasicAttackAbility(
        AbilityType type,
        string name,
        string description,
        int damage,
        int range,
        int priority,
        CharacterSpecAbilityActionAsset[] actionAssets) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            Type = type,
            EffectType = AbilityEffectType.Damage,
            TargetType = AbilityTargetType.Enemy,
            Name = name,
            Description = description,
            Priority = priority,
            Stats =
            [
                CreateAbilityStat(AbilityStatType.Damage, damage),
                CreateAbilityStat(AbilityStatType.Range, range)
            ],
            ActionAssets = actionAssets
        };

    private static CharacterSpecAbilityStat CreateAbilityStat(AbilityStatType type, int value) =>
        new()
        {
            StatType = type,
            Value = value
        };

    private static string GetSpecFolder(CharacterClassType classType, string specFolder) =>
        classType switch
        {
            CharacterClassType.Warrior => $"{AssetRoot}/warrior/{specFolder}",
            CharacterClassType.Mage => $"{AssetRoot}/mage/{specFolder}",
            _ => throw new ArgumentOutOfRangeException(nameof(classType), classType, null)
        };

    private static CharacterSpecActionAsset[] CreateWarriorActionAssets(
        CharacterSpecType type,
        string folder) =>
        type switch
        {
            CharacterSpecType.Berserker => CreateBerserkerActionAssets(folder),
            CharacterSpecType.Guardian => CreateGuardianActionAssets(folder),
            CharacterSpecType.Duelist => CreateDuelistActionAssets(folder),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

    private static CharacterSpecActionAsset[] CreateMageActionAssets(
        CharacterSpecType type,
        string folder) =>
        type switch
        {
            CharacterSpecType.Pyromancer => CreatePyromancerActionAssets(folder),
            CharacterSpecType.Luminary => CreateLuminaryActionAssets(folder),
            CharacterSpecType.Arcanist => CreateArcanistActionAssets(folder),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

    private static CharacterSpecAbility[] CreateWarriorAbilities(
        CharacterSpecType type,
        string folder,
        int meleeDamage) =>
        type switch
        {
            CharacterSpecType.Berserker =>
            [
                CreateBasicAttackAbility(
                    AbilityType.BasicMeleeAttack,
                    string.Empty,
                    string.Empty,
                    meleeDamage,
                    range: 1,
                    priority: 100,
                    [
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", 4, 0.1f),
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", 4, 0.1f, variant: 1),
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_3.png", 4, 0.1f, variant: 2),
                        CreateAbilityActionAsset(ActionType.Run_Attack, $"{folder}/Run_Attack.png", 4, 0.1f)
                    ])
            ],

            CharacterSpecType.Guardian =>
            [
                CreateBasicAttackAbility(
                    AbilityType.BasicMeleeAttack,
                    string.Empty,
                    string.Empty,
                    meleeDamage,
                    range: 1,
                    priority: 100,
                    [
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", 4, 0.1f),
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", 4, 0.1f, variant: 1),
                        CreateAbilityActionAsset(ActionType.Run_Attack, $"{folder}/Run_Attack.png", 4, 0.1f)
                    ])
            ],

            CharacterSpecType.Duelist =>
            [
                CreateBasicAttackAbility(
                    AbilityType.BasicMeleeAttack,
                    string.Empty,
                    string.Empty,
                    meleeDamage,
                    range: 1,
                    priority: 100,
                    [
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", 4, 0.1f),
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", 3, 0.1f, variant: 1),
                        CreateAbilityActionAsset(ActionType.Run_Attack, $"{folder}/Run_Attack.png", 4, 0.1f)
                    ])
            ],

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

    private static CharacterSpecAbility[] CreateMageAbilities(
        CharacterSpecType type,
        string folder,
        int? meleeDamage,
        int rangedDamage,
        int rangedRange) =>
        type switch
        {
            CharacterSpecType.Pyromancer =>
            [
                CreateBasicAttackAbility(
                    AbilityType.BasicMeleeAttack,
                    string.Empty,
                    string.Empty,
                    meleeDamage ?? throw new ArgumentNullException(nameof(meleeDamage)),
                    range: 1,
                    priority: 200,
                    [
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", 4, 0.1f),
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", 4, 0.1f, variant: 1)
                    ]),

                CreateBasicAttackAbility(
                    AbilityType.BasicRangedAttack,
                    "Поток пламени",
                    "Нельзя применить, если цель стоит на соседней клетке.",
                    rangedDamage,
                    range: rangedRange,
                    priority: 100,
                    [
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Flame_jet.png", 14, 0.2f)
                    ])
            ],

            CharacterSpecType.Luminary =>
            [
                CreateBasicAttackAbility(
                    AbilityType.BasicMeleeAttack,
                    string.Empty,
                    string.Empty,
                    meleeDamage ?? throw new ArgumentNullException(nameof(meleeDamage)),
                    range: 1,
                    priority: 200,
                    [
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", 10, 0.15f),
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", 4, 0.1f, variant: 1)
                    ]),

                CreateBasicAttackAbility(
                    AbilityType.BasicRangedAttack,
                    "Световой заряд",
                    "Нельзя применить, если цель стоит на соседней клетке.",
                    rangedDamage,
                    range: rangedRange,
                    priority: 100,
                    [
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Light_charge.png", 13, 0.2f)
                    ])
            ],

            CharacterSpecType.Arcanist =>
            [
                CreateBasicAttackAbility(
                    AbilityType.BasicRangedAttack,
                    "Арканный импульс",
                    "Можно применять как на расстоянии, так и вблизи.",
                    rangedDamage,
                    range: rangedRange,
                    priority: 100,
                    [
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", 7, 0.1f),
                        CreateAbilityActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", 9, 0.1f, variant: 1)
                    ])
            ],

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

    private static CharacterSpecActionAsset[] CreateBerserkerActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", 6, 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", 8, 0.1f),
        // CreateActionAsset(ActionType.Walk, $"{folder}/Run.png", 6, 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", 6, 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", 5, 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", 2, 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", 4, 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreateGuardianActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", 5, 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", 8, 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", 6, 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", 7, 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", 3, 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", 4, 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreateDuelistActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", 5, 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", 8, 0.1f),
        // CreateActionAsset(ActionType.Walk, $"{folder}/Run.png", 6, 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", 6, 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", 8, 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", 2, 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", 4, 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreatePyromancerActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", 7, 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", 6, 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", 8, 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", 9, 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", 3, 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", 6, 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreateLuminaryActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", 7, 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", 7, 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", 8, 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", 8, 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", 3, 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", 5, 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreateArcanistActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", 8, 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", 7, 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", 8, 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", 8, 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", 4, 0.15f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", 4, 0.1f)
    ];

    private static CharacterSpecActionAsset CreateActionAsset(
        ActionType actionType,
        string path,
        int frameCount,
        float animationSpeed,
        int? variant = null) =>
        new()
        {
            ActionType = actionType,
            Variant = variant ?? 0,
            Animation = new SpriteAnimation(
                new Uri(path, UriKind.Relative),
                frameCount,
                animationSpeed)
        };

    private static CharacterSpecAbilityActionAsset CreateAbilityActionAsset(
        ActionType actionType,
        string path,
        int frameCount,
        float animationSpeed,
        int? variant = null) =>
        new()
        {
            ActionType = actionType,
            Variant = variant ?? 0,
            Animation = new SpriteAnimation(
                new Uri(path, UriKind.Relative),
                frameCount,
                animationSpeed)
        };
}
