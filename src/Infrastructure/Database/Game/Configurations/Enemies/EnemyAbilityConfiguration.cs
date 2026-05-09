using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Enemies;

internal sealed class EnemyAbilityConfiguration : IEntityTypeConfiguration<EnemyAbility>
{
    public void Configure(EntityTypeBuilder<EnemyAbility> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .HasConversion<int>();

        builder.Property(e => e.EffectType)
            .HasConversion<int>();

        builder.Property(e => e.TargetType)
            .HasConversion<int>();

        builder.Property(e => e.Name)
            .HasMaxLength(64);

        builder.Property(e => e.Description)
            .HasMaxLength(512);

        builder.Property(e => e.Priority)
            .IsRequired();

        builder.HasIndex(e => new { e.EnemyId, e.Type })
            .IsUnique();

        builder.HasMany(e => e.Stats)
            .WithOne(s => s.Ability)
            .HasForeignKey(s => s.EnemyAbilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ActionAssets)
            .WithOne(a => a.Ability)
            .HasForeignKey(a => a.EnemyAbilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Enemy)
            .WithMany(s => s.Abilities)
            .HasForeignKey(e => e.EnemyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
