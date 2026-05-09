using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Enemies;

internal sealed class EnemyAbilityStatConfiguration : IEntityTypeConfiguration<EnemyAbilityStat>
{
    public void Configure(EntityTypeBuilder<EnemyAbilityStat> builder)
    {
        builder.HasKey(x => new { x.EnemyAbilityId, x.StatType });

        builder.Property(x => x.StatType)
            .HasConversion<int>();

        builder.Property(x => x.Value)
            .IsRequired();

        builder.HasOne(x => x.Ability)
            .WithMany(e => e.Stats)
            .HasForeignKey(x => x.EnemyAbilityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
