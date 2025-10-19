using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Enemies;

internal sealed class EnemyStatConfiguration : IEntityTypeConfiguration<EnemyStat>
{
    public void Configure(EntityTypeBuilder<EnemyStat> builder)
    {
        builder.HasKey(x => new { x.EnemyId, x.StatType });

        builder.Property(x => x.StatType)
            .HasConversion<int>();

        builder.HasOne(x => x.Enemy)
            .WithMany(e => e.Stats)
            .HasForeignKey(x => x.EnemyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
