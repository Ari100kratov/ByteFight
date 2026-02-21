using Domain;
using Domain.Game.Arenas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Arenas;

internal sealed class ArenaConfiguration : IEntityTypeConfiguration<Arena>
{
    public void Configure(EntityTypeBuilder<Arena> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(a => a.BackgroundAsset)
            .HasMaxLength(256);

        builder.Property(a => a.Description)
            .HasMaxLength(256);

        builder.Property(a => a.CreatedBy)
            .HasConversion(v => v.Value, v => new UserId(v))
            .IsRequired();

        builder.OwnsOne(x => x.StartPosition);
        builder.OwnsMany(x => x.BlockedPositions);

        //builder.HasQueryFilter(a => a.IsActive);
    }
}
