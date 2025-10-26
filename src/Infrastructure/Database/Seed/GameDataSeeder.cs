using Application.Abstractions.Data;
using Domain;
using Domain.Game.Actions;
using Domain.Game.Arenas;
using Domain.Game.Arenas.ArenaEnemies;
using Domain.Game.CharacterClasses;
using Domain.Game.Enemies;
using Domain.Game.GameModes;
using Domain.Game.Stats;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seed;

public class GameDataSeeder(IGameDbContext dbContext)
{
    public async Task Seed(SeedContext seed, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Arenas.AnyAsync(cancellationToken))
        {
            return;
        }

        AddArenas(seed);

        AddEnemies(seed);
        AddArenaEnemies(seed);

        AddCharacterClasses(seed);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void AddArenas(SeedContext seed)
    {
        var trainingArena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Тренировочная поляна",
            Description = "Спокойная зелёная поляна на окраине деревни, где новички делают первые шаги в бою.",
            GridWidth = 8,
            GridHeight = 8,
            BackgroundAsset = "arenas/training-ground.png",
            GameModes = [GameModeType.Training],
            IsActive = true,
            CreatedBy = new UserId(seed.AdminId),
            CreatedAt = DateTime.UtcNow
        };

        var banditCampArena = new Arena
        {
            Id = Guid.CreateVersion7(),
            Name = "Заброшенный лагерь бандитов",
            Description = "Разрушенные палатки и кострища напоминают о недавних боях. В воздухе витает дым и запах железа. Отличное место, чтобы проверить свои силы против более опытных противников.",
            GridWidth = 15,
            GridHeight = 15,
            BackgroundAsset = "arenas/bandit-camp.png",
            GameModes = [GameModeType.Training],
            IsActive = true,
            CreatedBy = new UserId(seed.AdminId),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Arenas.AddRange(trainingArena, banditCampArena);

        seed.Arena_1 = trainingArena.Id;
        seed.Arena_2 = banditCampArena.Id;
    }

    private void AddEnemies(SeedContext seed)
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
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Walk,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Walk.png", UriKind.Relative),
                        frameCount: 7,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Run,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Run.png", UriKind.Relative),
                        frameCount: 6,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Attack_1.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 1,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Attack_2.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 2,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Attack_3.png", UriKind.Relative),
                        frameCount: 3,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Run_Attack,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Run_Attack.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Jump,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Jump.png", UriKind.Relative),
                        frameCount: 8,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Hurt,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Hurt.png", UriKind.Relative),
                        frameCount: 2,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Dead,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Dead.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                }
            ]
        };

        dbContext.Enemies.Add(orcWarrior);
        seed.Orc_Warrior = orcWarrior.Id;
    }

    private void AddArenaEnemies(SeedContext seed)
    {
        var arenaEnemies = new List<ArenaEnemy>
        {
            new()
            {
                Id = Guid.CreateVersion7(),
                ArenaId = seed.Arena_1,
                EnemyId = seed.Orc_Warrior,
                Position = new Position(2, 5)
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ArenaId = seed.Arena_1,
                EnemyId = seed.Orc_Warrior,
                Position = new Position(6, 6)
            }
        };

        dbContext.ArenaEnemies.AddRange(arenaEnemies);
    }

    private void AddCharacterClasses(SeedContext seed)
    {
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
                new() { StatType = StatType.MoveRange, Value = 1 },
            ],
            ActionAssets =
            [
                new()
                {
                    ActionType = ActionType.Idle,
                    Animation = new SpriteAnimation(
                        new Uri("classes/warrior-1/Idle.png", UriKind.Relative),
                        frameCount: 6,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Walk,
                    Animation = new SpriteAnimation(
                        new Uri("classes/warrior-1/Walk.png", UriKind.Relative),
                        frameCount: 8,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Run,
                    Animation = new SpriteAnimation(
                        new Uri("classes/warrior-1/Run.png", UriKind.Relative),
                        frameCount: 6,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Animation = new SpriteAnimation(
                        new Uri("classes/warrior-1/Attack_1.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 1,
                    Animation = new SpriteAnimation(
                        new Uri("classes/warrior-1/Attack_2.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Attack,
                    Variant = 2,
                    Animation = new SpriteAnimation(
                        new Uri("classes/warrior-1/Attack_3.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Run_Attack,
                    Animation = new SpriteAnimation(
                        new Uri("classes/warrior-1/Run_Attack.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Jump,
                    Animation = new SpriteAnimation(
                        new Uri("enemies/orc-warrior/Jump.png", UriKind.Relative),
                        frameCount: 5,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Hurt,
                    Animation = new SpriteAnimation(
                        new Uri("classes/warrior-1/Hurt.png", UriKind.Relative),
                        frameCount: 2,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                },
                new()
                {
                    ActionType = ActionType.Dead,
                    Animation = new SpriteAnimation(
                        new Uri("classes/warrior-1/Dead.png", UriKind.Relative),
                        frameCount: 4,
                        animationSpeed: 0.1f,
                        scaleX: -1f,
                        scaleY: 1f)
                }
            ]
        };

        dbContext.CharacterClasses.Add(warrior);
        seed.Class_Warrior = warrior.Id;
    }
}
