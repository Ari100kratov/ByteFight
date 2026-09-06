using Domain.Game.Arenas.ArenaTerrainCells;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Arenas;

internal sealed class ArenaTerrainCellConfiguration
    : IEntityTypeConfiguration<ArenaTerrainCell>
{
    public void Configure(EntityTypeBuilder<ArenaTerrainCell> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Terrain)
            .HasConversion<int>();

        builder.OwnsOne(x => x.Position);

        builder.HasOne(x => x.Arena)
            .WithMany(a => a.TerrainCells)
            .HasForeignKey(x => x.ArenaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
