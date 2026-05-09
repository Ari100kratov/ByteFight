using Application.Abstractions.Data;
using Domain.Game.Abilities;
using Domain.Game.Actions;
using Domain.Game.Enemies;
using Domain.Game.Stats;
using Domain.ValueObjects;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal static class EnemiesSeeder
{
    private const string AssetRoot = "enemies";

    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        Enemy orcWarrior = CreateOrcWarrior();
        Enemy orcBerserker = CreateOrcBerserker();
        Enemy orcShaman = CreateOrcShaman();
        Enemy skeleton = CreateSkeleton();

        dbContext.Enemies.AddRange(orcWarrior, orcBerserker, orcShaman, skeleton);

        seed.Orc_Warrior = orcWarrior.Id;
        seed.Orc_Berserker = orcBerserker.Id;
        seed.Orc_Shaman = orcShaman.Id;
        seed.Skeleton = skeleton.Id;
    }

    private static Enemy CreateOrcWarrior()
    {
        const string folder = $"{AssetRoot}/orc-warrior";

        return new Enemy
        {
            Id = Guid.CreateVersion7(),
            Name = "Орк-воин",
            Description = "Орк-воин решает большинство проблем топором.\r\n" +
                "Если проблема не решилась — значит, топором махнули недостаточно сильно.\r\n" +
                "Простой, упрямый и опасный, особенно когда понял, в какую сторону бежать.",
            Stats =
            [
                CreateStat(StatType.Health, 105),
                CreateStat(StatType.MoveRange, 2),
            ],
            ActionAssets = CreateOrcWarriorActionAssets(folder),
            Abilities =
            [
                CreateBasicMeleeAttack(
                    name: string.Empty,
                    description: string.Empty,
                    damage: 16,
                    folder: folder,
                    frameCountByAsset:
                    [
                        ("Attack_1.png", 4, 0),
                        ("Attack_2.png", 4, 1),
                        ("Attack_3.png", 3, 2),
                    ])
            ]
        };
    }

    private static Enemy CreateOrcBerserker()
    {
        const string folder = $"{AssetRoot}/orc-berserk";

        return new Enemy
        {
            Id = Guid.CreateVersion7(),
            Name = "Орк-берсерк",
            Description = "Орк-берсерк не входит в бой — он в него врезается.\r\n" +
                "Где остальные видят опасность, он видит повод ускориться.\r\n" +
                "После его атаки на поле боя обычно остаются враги, вмятины и очень удивлённые союзники.",
            Stats =
            [
                CreateStat(StatType.Health, 80),
                CreateStat(StatType.MoveRange, 2),
            ],
            ActionAssets = CreateOrcBerserkerActionAssets(folder),
            Abilities =
            [
                CreateBasicMeleeAttack(
                    name: string.Empty,
                    description: string.Empty,
                    damage: 22,
                    folder: folder,
                    frameCountByAsset:
                    [
                        ("Attack_1.png", 4, 0),
                        ("Attack_2.png", 5, 1),
                    ])
            ]
        };
    }

    private static Enemy CreateOrcShaman()
    {
        const string folder = $"{AssetRoot}/orc-shaman";

        return new Enemy
        {
            Id = Guid.CreateVersion7(),
            Name = "Орк-шаман",
            Description = "Орк-шаман держится позади, потому что мудрость предков подсказала ему:\r\n" +
                "в первых рядах слишком часто заканчиваются родственники.\r\n" +
                "Он лечит союзников, ворчит на духов и делает вид, что всё идёт по ритуальному плану.",
            Stats =
            [
                CreateStat(StatType.Health, 65),
                CreateStat(StatType.MoveRange, 2),
            ],
            ActionAssets = CreateOrcShamanActionAssets(folder),
            Abilities =
            [
                CreateBasicMeleeAttack(
                    name: string.Empty,
                    description: string.Empty,
                    damage: 10,
                    folder: folder,
                    frameCountByAsset:
                    [
                        ("Attack_1.png", 4, 0),
                        ("Attack_2.png", 2, 1),
                    ]),

                CreateHealAbility(
                    name: "Зов предков",
                    description: "Шаман взывает к духам предков, направляя их силу в союзника.\r\n" +
                        "Духовная энергия мгновенно затягивает его раны.",
                    heal: 24,
                    range: 4,
                    folder: folder,
                    frameCountByAsset:
                    [
                        ("Magic_2.png", 6, 0),
                    ])
            ]
        };
    }

    private static Enemy CreateSkeleton()
    {
        const string folder = $"{AssetRoot}/skeleton";

        return new Enemy
        {
            Id = Guid.CreateVersion7(),
            Name = "Скелет",
            Description = "Когда-то он был обычным стражником. Теперь у него нет ни страха, ни усталости,\r\n" +
                "ни уважительной причины оставаться лежать в могиле.\r\n" +
                "Двигается медленно, гремит костями громко, а бьёт неожиданно уверенно.",
            Stats =
            [
                CreateStat(StatType.Health, 55),
                CreateStat(StatType.MoveRange, 2),
            ],
            ActionAssets = CreateSkeletonActionAssets(folder),
            Abilities =
            [
                CreateBasicMeleeAttack(
                    name: string.Empty,
                    description: string.Empty,
                    damage: 16,
                    folder: folder,
                    scaleX: 0.8f,
                    scaleY: 0.8f,
                    frameCountByAsset:
                    [
                        ("Attack_1.png", 7, 0),
                        ("Attack_2.png", 4, 1),
                        ("Special_attack.png", 5, 2),
                    ])
            ]
        };
    }

    private static EnemyStat CreateStat(StatType type, int value) =>
        new()
        {
            StatType = type,
            Value = value
        };

    private static EnemyAbility CreateBasicMeleeAttack(
        string name,
        string description,
        int damage,
        string folder,
        (string FileName, int FrameCount, int Variant)[] frameCountByAsset,
        float scaleX = 1f,
        float scaleY = 1f) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            Type = AbilityType.BasicMeleeAttack,
            EffectType = AbilityEffectType.Damage,
            TargetType = AbilityTargetType.Enemy,
            Name = name,
            Description = description,
            Priority = 100,
            Stats =
            [
                CreateAbilityStat(AbilityStatType.Damage, damage),
                CreateAbilityStat(AbilityStatType.Range, 1),
            ],
            ActionAssets =
            [
                .. frameCountByAsset.Select(x =>
                    CreateAbilityActionAsset(
                        ActionType.Attack,
                        $"{folder}/{x.FileName}",
                        x.FrameCount,
                        animationSpeed: 0.1f,
                        variant: x.Variant,
                        scaleX,
                        scaleY))
            ]
        };

    private static EnemyAbility CreateHealAbility(
        string name,
        string description,
        int heal,
        int range,
        string folder,
        (string FileName, int FrameCount, int Variant)[] frameCountByAsset,
        float scaleX = 1f,
        float scaleY = 1f) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            Type = AbilityType.Healing,
            EffectType = AbilityEffectType.Healing,
            TargetType = AbilityTargetType.Ally,
            Name = name,
            Description = description,
            Priority = 80,
            Stats =
            [
                CreateAbilityStat(AbilityStatType.Healing, heal),
                CreateAbilityStat(AbilityStatType.Range, range),
            ],
            ActionAssets =
            [
                .. frameCountByAsset.Select(x =>
                    CreateAbilityActionAsset(
                        ActionType.Cast,
                        $"{folder}/{x.FileName}",
                        x.FrameCount,
                        animationSpeed: 0.1f,
                        variant: x.Variant,
                        scaleX,
                        scaleY))
            ]
        };

    private static EnemyAbilityStat CreateAbilityStat(AbilityStatType type, int value) =>
        new()
        {
            StatType = type,
            Value = value
        };

    private static EnemyActionAsset[] CreateOrcWarriorActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 5, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 7, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 6, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 2, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 4, animationSpeed: 0.1f)
    ];

    private static EnemyActionAsset[] CreateOrcBerserkerActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 5, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 7, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 6, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 5, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 2, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 4, animationSpeed: 0.1f)
    ];

    private static EnemyActionAsset[] CreateOrcShamanActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 5, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 7, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 6, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 6, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 2, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 5, animationSpeed: 0.1f)
    ];

    private static EnemyActionAsset[] CreateSkeletonActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 7, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 8, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 7, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 10, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 3, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 3, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f)
    ];

    private static EnemyActionAsset CreateActionAsset(
        ActionType actionType,
        string path,
        int frameCount,
        float animationSpeed,
        int? variant = null,
        float scaleX = 1f,
        float scaleY = 1f) =>
        new()
        {
            ActionType = actionType,
            Variant = variant ?? 0,
            Animation = new SpriteAnimation(
                new Uri(path, UriKind.Relative),
                frameCount,
                animationSpeed,
                scaleX,
                scaleY)
        };

    private static EnemyAbilityActionAsset CreateAbilityActionAsset(
        ActionType actionType,
        string path,
        int frameCount,
        float animationSpeed,
        int variant,
        float scaleX = 1f,
        float scaleY = 1f) =>
        new()
        {
            ActionType = actionType,
            Variant = variant,
            Animation = new SpriteAnimation(
                new Uri(path, UriKind.Relative),
                frameCount,
                animationSpeed,
                scaleX,
                scaleY)
        };
}
