using Domain.Game.Arenas.ArenaPlacedItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Arenas;

internal sealed class ArenaPlacedItemConfiguration : IEntityTypeConfiguration<ArenaPlacedItem>
{
    public void Configure(EntityTypeBuilder<ArenaPlacedItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ArenaId)
            .IsRequired();

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.OwnsOne(x => x.Position);

        builder.HasOne(x => x.Arena)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ArenaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ArenaId,
            x.ItemId
        });
    }
}
