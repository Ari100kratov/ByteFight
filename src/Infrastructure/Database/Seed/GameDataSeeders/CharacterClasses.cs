using Application.Abstractions.Data;
using Domain.Game.Actions;
using Domain.Game.CharacterClasses;
using Domain.Game.Stats;
using Domain.ValueObjects;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal class CharacterClasses
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        string assetFolder = "classes";
        string warriorFolder = "warrior-1";

        var warrior = new CharacterClass
        {
            Id = Guid.CreateVersion7(),
            Type = CharacterClassType.Warrior,
            Name = "Воин",
            Description = "Сильный боец ближнего боя, мастер владения оружием.",
            Stats =
            [
                new() { StatType = StatType.Health, Value = 150 },
                new() { StatType = StatType.Attack, Value = 30 },
                new() { StatType = StatType.AttackRange, Value = 1 },
                new() { StatType = StatType.MoveRange, Value = 1 },
            ],
            ActionAssets =
            [
                new()
                {
                    ActionType = ActionType.Idle,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Idle.png", UriKind.Relative),
                        frameCount: 6,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Walk,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Walk.png", UriKind.Relative),
                        frameCount: 8,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Run,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Run.png", UriKind.Relative),
                        frameCount: 6,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Attack_1.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 1,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Attack_2.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 2,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Attack_3.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Run_Attack,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Run_Attack.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Jump,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Jump.png", UriKind.Relative),
                        frameCount: 5,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Hurt,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Hurt.png", UriKind.Relative),
                        frameCount: 2,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Dead,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{warriorFolder}/Dead.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                }
            ]
        };

        string wizardFolder = "wizard-fire";

        var mage = new CharacterClass
        {
            Id = Guid.CreateVersion7(),
            Type = CharacterClassType.Mage,
            Name = "Маг",
            Description = "Могущественный заклинатель, владеющий разрушительной магией стихий. Слабо защищён в ближнем бою, но способен уничтожить врагов на расстоянии.",
            Stats =
            [
                new() { StatType = StatType.Health, Value = 90 },
                new() { StatType = StatType.Attack, Value = 50 },
                new() { StatType = StatType.AttackRange, Value = 3 },
                new() { StatType = StatType.MoveRange, Value = 2 },
                new() { StatType = StatType.Mana, Value = 100 }
            ],
            ActionAssets =
            [
                new()
                {
                    ActionType = ActionType.Idle,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{wizardFolder}/Idle.png", UriKind.Relative),
                        frameCount: 7,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Walk,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{wizardFolder}/Walk.png", UriKind.Relative),
                        frameCount: 6,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Run,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{wizardFolder}/Run.png", UriKind.Relative),
                        frameCount: 8,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{wizardFolder}/Flame_jet.png", UriKind.Relative),
                        frameCount: 14,
                        animationSpeed: 0.2f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 1,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{wizardFolder}/Attack_1.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 2,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{wizardFolder}/Attack_2.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Jump,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{wizardFolder}/Jump.png", UriKind.Relative),
                        frameCount: 9,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Hurt,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{wizardFolder}/Hurt.png", UriKind.Relative),
                        frameCount: 3,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Dead,
                    Animation = new SpriteAnimation(
                        new Uri($"{assetFolder}/{wizardFolder}/Dead.png", UriKind.Relative),
                        frameCount: 6,
                        animationSpeed: 0.1f)
                }
            ]
        };

        dbContext.CharacterClasses.AddRange(warrior, mage);
        seed.Class_Warrior = warrior.Id;
        seed.Class_Mage = mage.Id;
    }
}
