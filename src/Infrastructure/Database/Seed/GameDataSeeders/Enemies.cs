using Application.Abstractions.Data;
using Domain.Game.Actions;
using Domain.Game.Enemies;
using Domain.Game.Stats;
using Domain.ValueObjects;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal class Enemies
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        var orcWarrior = new Enemy
        {
            Id = Guid.CreateVersion7(),
            Name = "Орк-воин",
            Description = "Грубый, но выносливый воин, вооружённый тупым мечом.",
            Stats =
            [
                new() { StatType = StatType.Health, Value = 100 },
                new() { StatType = StatType.Attack, Value = 15 },
                new() { StatType = StatType.AttackRange, Value = 1 },
                new() { StatType = StatType.MoveRange, Value = 1 },
            ],
            ActionAssets =
            [
                new()
                {
                    ActionType = ActionType.Idle,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Idle.png", UriKind.Relative),
                        frameCount: 5,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Walk,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Walk.png", UriKind.Relative),
                        frameCount: 7,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Run,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Run.png", UriKind.Relative),
                        frameCount: 6,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Attack_1.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 1,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Attack_2.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 2,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Attack_3.png", UriKind.Relative),
                        frameCount: 3,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Run_Attack,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Run_Attack.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Jump,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Jump.png", UriKind.Relative),
                        frameCount: 8,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Hurt,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Hurt.png", UriKind.Relative),
                        frameCount: 2,
                        animationSpeed: 0.1f)
                },
                new()
                {
                    ActionType = ActionType.Dead,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Dead.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f)
                }
            ]
        };

        dbContext.Enemies.Add(orcWarrior);
        seed.Orc_Warrior = orcWarrior.Id;
    }
}
