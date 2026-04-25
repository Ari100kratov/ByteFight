using Application.Abstractions.Data;
using Domain.Game.Actions;
using Domain.Game.CharacterClasses;
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
            health: 150,
            attack: 40,
            attackRange: 1,
            moveRange: 2,
            specIdSetter: id => seed.Spec_Warrior_Berserker = id);

        CharacterSpec guardian = CreateWarriorSpec(
            warrior,
            CharacterSpecType.Guardian,
            "Страж",
            "Выносливый защитник, способный долго держать линию фронта.",
            folder: "guardian",
            health: 220,
            attack: 24,
            attackRange: 1,
            moveRange: 1,
            specIdSetter: id => seed.Spec_Warrior_Guardian = id);

        CharacterSpec duelist = CreateWarriorSpec(
            warrior,
            CharacterSpecType.Duelist,
            "Дуэлянт",
            "Подвижный мастер ближнего боя, побеждающий за счёт темпа и точности.",
            folder: "duelist",
            health: 170,
            attack: 32,
            attackRange: 1,
            moveRange: 2,
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
            health: 105,
            attack: 46,
            attackRange: 3,
            moveRange: 1,
            mana: 100,
            specIdSetter: id => seed.Spec_Mage_Pyromancer = id);

        CharacterSpec luminary = CreateMageSpec(
            mage,
            CharacterSpecType.Luminary,
            "Люминар",
            "Маг света, использующий концентрированную энергию для точечных атак на расстоянии.",
            folder: "luminary",
            health: 110,
            attack: 42,
            attackRange: 3,
            moveRange: 1,
            mana: 115,
            specIdSetter: id => seed.Spec_Mage_Luminary = id);

        CharacterSpec arcanist = CreateMageSpec(
            mage,
            CharacterSpecType.Arcanist,
            "Арканист",
            "Универсальный маг тайной школы, полагающийся на запас маны и гибкость.",
            folder: "arcanist",
            health: 115,
            attack: 38,
            attackRange: 2,
            moveRange: 1,
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
        int attack,
        int attackRange,
        int moveRange,
        Action<Guid> specIdSetter)
    {
        CharacterSpec spec = new()
        {
            Id = Guid.CreateVersion7(),
            ClassId = characterClass.Id,
            Type = type,
            Name = name,
            Description = description,
            Stats =
            [
                CreateStat(StatType.Health, health),
                CreateStat(StatType.Attack, attack),
                CreateStat(StatType.AttackRange, attackRange),
                CreateStat(StatType.MoveRange, moveRange)
            ],
            ActionAssets = CreateWarriorActionAssets(
                type,
                GetSpecFolder(CharacterClassType.Warrior, folder))
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
        int attack,
        int attackRange,
        int moveRange,
        int mana,
        Action<Guid> specIdSetter)
    {
        CharacterSpec spec = new()
        {
            Id = Guid.CreateVersion7(),
            ClassId = characterClass.Id,
            Type = type,
            Name = name,
            Description = description,
            Stats =
            [
                CreateStat(StatType.Health, health),
                CreateStat(StatType.Attack, attack),
                CreateStat(StatType.AttackRange, attackRange),
                CreateStat(StatType.MoveRange, moveRange),
                CreateStat(StatType.Mana, mana)
            ],
            ActionAssets = CreateMageActionAssets(
                type,
                GetSpecFolder(CharacterClassType.Mage, folder))
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

    private static CharacterSpecActionAsset[] CreateBerserkerActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 6, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 6, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", frameCount: 4, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", frameCount: 4, animationSpeed: 0.1f, variant: 1),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_3.png", frameCount: 4, animationSpeed: 0.1f, variant: 2),
        CreateActionAsset(ActionType.Run_Attack, $"{folder}/Run_Attack.png", frameCount: 4, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 5, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 2, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 4, animationSpeed: 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreateGuardianActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 5, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 6, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", frameCount: 4, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", frameCount: 4, animationSpeed: 0.1f, variant: 1),
        CreateActionAsset(ActionType.Run_Attack, $"{folder}/Run_Attack.png", frameCount: 4, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 7, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 3, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 4, animationSpeed: 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreateDuelistActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 5, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 6, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", frameCount: 4, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", frameCount: 3, animationSpeed: 0.1f, variant: 1),
        CreateActionAsset(ActionType.Run_Attack, $"{folder}/Run_Attack.png", frameCount: 4, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 2, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 4, animationSpeed: 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreatePyromancerActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 7, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 6, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Flame_jet.png", frameCount: 14, animationSpeed: 0.2f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", frameCount: 4, animationSpeed: 0.1f, variant: 1),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", frameCount: 4, animationSpeed: 0.1f, variant: 2),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 9, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 3, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 6, animationSpeed: 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreateLuminaryActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 7, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 7, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Light_charge.png", frameCount: 13, animationSpeed: 0.2f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", frameCount: 10, animationSpeed: 0.15f, variant: 1),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", frameCount: 4, animationSpeed: 0.1f, variant: 2),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 3, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 5, animationSpeed: 0.1f)
    ];

    private static CharacterSpecActionAsset[] CreateArcanistActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 7, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", frameCount: 7, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", frameCount: 9, animationSpeed: 0.1f, variant: 1),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 4, animationSpeed: 0.15f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 4, animationSpeed: 0.1f)
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
}
