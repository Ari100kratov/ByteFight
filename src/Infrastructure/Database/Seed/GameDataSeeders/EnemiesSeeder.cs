using Application.Abstractions.Data;
using Domain.Game.Abilities;
using Domain.Game.Enemies;
using Domain.Game.Stats;

namespace Infrastructure.Database.Seed.GameDataSeeders;

/// <summary>
/// Древнеславянская нечисть и зверьё. Игровые значения способностей —
/// из <see cref="AbilityCatalog"/>, здесь задаётся состав и здоровье.
/// </summary>
internal static class EnemiesSeeder
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        Enemy ghoul = CreateGhoul();
        Enemy leshy = CreateLeshy();
        Enemy kikimora = CreateKikimora();
        Enemy skeleton = CreateSkeletonWarrior();
        Enemy volkolak = CreateVolkolak();

        dbContext.Enemies.AddRange(ghoul, leshy, kikimora, skeleton, volkolak);

        seed.Ghoul = ghoul.Id;
        seed.Leshy = leshy.Id;
        seed.Kikimora = kikimora.Id;
        seed.SkeletonWarrior = skeleton.Id;
        seed.Volkolak = volkolak.Id;
    }

    private static Enemy CreateGhoul()
    {
        return new Enemy
        {
            Id = Guid.CreateVersion7(),
            Name = "Упырь",
            Description = "Погостный упырь: голоден, цепок и обидчив.\r\n" +
                "Когти рвут, дыхание воняет, а терпение кончилось ещё при жизни.",
            Stats =
            [
                CreateStat(StatType.Health, 110),
                CreateStat(StatType.MoveRange, 3),
                CreateStat(StatType.Initiative, 6),
                CreateStat(StatType.Armor, 2)
            ],
            ActionAssets = [],
            Abilities = [CreateAbility(AbilityType.GhoulClawStrike)]
        };
    }

    private static Enemy CreateLeshy()
    {
        return new Enemy
        {
            Id = Guid.CreateVersion7(),
            Name = "Леший",
            Description = "Хозяин чащи: мох вместо бороды, корни вместо ног.\r\n" +
                "Скрутит тропу, запутает след и уйдёт в папоротник, посмеиваясь.",
            Stats =
            [
                CreateStat(StatType.Health, 160),
                CreateStat(StatType.MoveRange, 2),
                CreateStat(StatType.Initiative, 5),
                CreateStat(StatType.Armor, 4)
            ],
            ActionAssets = [],
            Abilities =
            [
                CreateAbility(AbilityType.LeshyRootVines),
                CreateAbility(AbilityType.LeshyRegeneration),
                CreateAbility(AbilityType.BasicMeleeAttack)
            ]
        };
    }

    private static Enemy CreateKikimora()
    {
        return new Enemy
        {
            Id = Guid.CreateVersion7(),
                       Name = "Кикимора",
            Description = "Болотная кикимора: шипит, плюётся и обожает чужое горе.\r\n" +
                "Держится на кочку подальше, но яд у неё — знатный.",
            Stats =
            [
                CreateStat(StatType.Health, 85),
                CreateStat(StatType.MoveRange, 3),
                CreateStat(StatType.Initiative, 8),
                CreateStat(StatType.Armor, 1)
            ],
            ActionAssets = [],
            Abilities = [CreateAbility(AbilityType.KikimoraPoisonSpit)]
        };
    }

    private static Enemy CreateSkeletonWarrior()
    {
        return new Enemy
        {
            Id = Guid.CreateVersion7(),
            Name = "Скелет-ратник",
            Description = "Когда-то он был обычным стражником. Теперь у него нет ни страха, ни усталости,\r\n" +
                "ни уважительной причины оставаться лежать в могиле.",
            Stats =
            [
                CreateStat(StatType.Health, 60),
                CreateStat(StatType.MoveRange, 2),
                CreateStat(StatType.Initiative, 5),
                CreateStat(StatType.Armor, 3)
            ],
            ActionAssets = [],
            Abilities = [CreateAbility(AbilityType.SkeletonBoneSlash)]
        };
    }

    private static Enemy CreateVolkolak()
    {
        return new Enemy
        {
            Id = Guid.CreateVersion7(),
            Name = "Волколак",
            Description = "Человек, которого лес переломил в волка.\r\n" +
                "Прыжок — и вот он уже у горла, а не за три гекса.",
            Stats =
            [
                CreateStat(StatType.Health, 95),
                CreateStat(StatType.MoveRange, 4),
                CreateStat(StatType.Initiative, 11),
                CreateStat(StatType.Armor, 1)
            ],
            ActionAssets = [],
            Abilities =
            [
                CreateAbility(AbilityType.WolfDashBite),
                CreateAbility(AbilityType.BasicMeleeAttack)
            ]
        };
    }

    private static EnemyStat CreateStat(StatType type, int value) =>
        new()
        {
            StatType = type,
            Value = value
        };

    private static EnemyAbility CreateAbility(AbilityType type, int priority = 0)
    {
        AbilityDefinition definition = AbilityCatalog.Get(type);

        return new EnemyAbility
        {
            Id = Guid.CreateVersion7(),
            Type = definition.Type,
            EffectType = definition.EffectType,
            TargetType = definition.TargetType,
            Name = definition.Name,
            Description = definition.Description,
            Priority = priority,
            Stats = [],
            ActionAssets = []
        };
    }
}
