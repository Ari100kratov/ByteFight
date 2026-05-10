using Domain.Game.ArenaItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Arenas;

internal sealed class ArenaItemConfiguration : IEntityTypeConfiguration<ArenaItem>
{
    public void Configure(EntityTypeBuilder<ArenaItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Description)
            .HasMaxLength(256);

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();

        builder.OwnsOne(x => x.Sprite, spriteBuilder =>
        {
            spriteBuilder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(256);

            spriteBuilder.Property(x => x.FrameCount)
                .IsRequired();

            spriteBuilder.Property(x => x.AnimationSpeed)
                .IsRequired();

            spriteBuilder.OwnsOne(x => x.Scale);
        });

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
