using Domain.Game.Arenas.ArenaEnemies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Arenas;

public sealed class ArenaEnemyConfiguration : IEntityTypeConfiguration<ArenaEnemy>
{
    public void Configure(EntityTypeBuilder<ArenaEnemy> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Position);

        builder.HasOne(x => x.Arena)
            .WithMany(a => a.Enemies)
            .HasForeignKey(x => x.ArenaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Enemy)
            .WithMany()
            .HasForeignKey(x => x.EnemyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
