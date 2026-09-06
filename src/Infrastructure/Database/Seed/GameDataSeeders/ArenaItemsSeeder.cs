using Application.Abstractions.Data;
using Domain.Game.ArenaItems;
using Domain.Game.Arenas.ArenaPlacedItems;
using Domain.ValueObjects;

namespace Infrastructure.Database.Seed.GameDataSeeders;

internal static class ArenaItemsSeeder
{
    public static void Seed(SeedContext seed, IGameDbContext dbContext)
    {
        var healingPotion = new ArenaItem
        {
            Id = Guid.CreateVersion7(),
            Name = "Лечебное зелье",
            Description =
                "Восстанавливает здоровье и, возможно, несколько жизненных решений. " +
                "На вкус как болотная вода с привкусом отчаяния, но работает отлично.",
            Type = ArenaItemType.HealingPotion,
            Sprite = new SpriteAnimation(
                new Uri("arena-items/healing-potion/healing-potion.png", UriKind.Relative),
                frameCount: 1,
                animationSpeed: 1f,
                scaleX: 1f,
                scaleY: 1f)
        };

        healingPotion.SetValue(100);

        dbContext.ArenaItems.Add(healingPotion);

        dbContext.ArenaPlacedItems.AddRange(
            CreatePlacedItem(
                seed.Graveyard_Arena,
                healingPotion.Id,
                new Position(8, 5)),

            CreatePlacedItem(
                seed.Swamp_Arena,
                healingPotion.Id,
                new Position(5, 5))
        );
    }

    private static ArenaPlacedItem CreatePlacedItem(
        Guid arenaId,
        Guid itemId,
        Position position)
    {
        return new ArenaPlacedItem
        {
            Id = Guid.CreateVersion7(),
            ArenaId = arenaId,
            ItemId = itemId,
            Position = position
        };
    }
}
