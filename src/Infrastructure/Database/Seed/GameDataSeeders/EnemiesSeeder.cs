using Application.Abstractions.Data;
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
        Enemy skeleton = CreateSkeleton();

        dbContext.Enemies.AddRange(orcWarrior, skeleton);

        seed.Orc_Warrior = orcWarrior.Id;
        seed.Skeleton = skeleton.Id;
    }

    private static Enemy CreateOrcWarrior() =>
        new()
        {
            Id = Guid.CreateVersion7(),
            Name = "Орк-воин",
            Description = "Слышал о стратегии. Даже пытался однажды её использовать.\r\n" +
                "С тех пор предпочитает старый проверенный способ —\r\n" +
                "бежать вперёд и размахивать топором, пока кто-нибудь не перестанет двигаться.",
            Stats =
            [
                CreateStat(StatType.Health, 100),
                CreateStat(StatType.Attack, 20),
                CreateStat(StatType.AttackRange, 1),
                CreateStat(StatType.MoveRange, 1),
            ],
            ActionAssets = CreateOrcWarriorActionAssets($"{AssetRoot}/orc-warrior")
        };

    private static Enemy CreateSkeleton() =>
        new()
        {
            Id = Guid.CreateVersion7(),
            Name = "Скелет",
            Description = "Когда-то он был обычным стражником. Теперь у него нет ни страха, ни усталости,\r\n" +
                "ни уважительной причины оставаться лежать в могиле.\r\n" +
                "Двигается медленно, гремит костями громко, а бьёт неожиданно уверенно.",
            Stats =
            [
                CreateStat(StatType.Health, 55),
                CreateStat(StatType.Attack, 16),
                CreateStat(StatType.AttackRange, 1),
                CreateStat(StatType.MoveRange, 1),
            ],
            ActionAssets = CreateSkeletonActionAssets($"{AssetRoot}/skeleton")
        };

    private static EnemyStat CreateStat(StatType type, int value) =>
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
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", frameCount: 4, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", frameCount: 4, animationSpeed: 0.1f, variant: 1),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_3.png", frameCount: 3, animationSpeed: 0.1f, variant: 2),
        CreateActionAsset(ActionType.Run_Attack, $"{folder}/Run_Attack.png", frameCount: 4, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Jump, $"{folder}/Jump.png", frameCount: 8, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Hurt, $"{folder}/Hurt.png", frameCount: 2, animationSpeed: 0.1f),
        CreateActionAsset(ActionType.Dead, $"{folder}/Dead.png", frameCount: 4, animationSpeed: 0.1f)
    ];

    private static EnemyActionAsset[] CreateSkeletonActionAssets(string folder) =>
    [
        CreateActionAsset(ActionType.Idle, $"{folder}/Idle.png", frameCount: 7, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Walk, $"{folder}/Walk.png", frameCount: 8, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Run, $"{folder}/Run.png", frameCount: 7, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_1.png", frameCount: 7, animationSpeed: 0.1f, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Attack_2.png", frameCount: 4, animationSpeed: 0.1f, variant: 1, scaleX: 0.8f, scaleY: 0.8f),
        CreateActionAsset(ActionType.Attack, $"{folder}/Special_attack.png", frameCount: 5, animationSpeed: 0.1f, variant: 2, scaleX: 0.8f, scaleY: 0.8f),
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
}
