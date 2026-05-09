using Domain.Game.CharacterSpecAbilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.CharacterSpecAbilities;

internal sealed class CharacterSpecAbilityConfiguration : IEntityTypeConfiguration<CharacterSpecAbility>
{
    public void Configure(EntityTypeBuilder<CharacterSpecAbility> builder)
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

        builder.HasIndex(e => new { e.CharacterSpecId, e.Type })
            .IsUnique();

        builder.HasMany(e => e.Stats)
            .WithOne(s => s.Ability)
            .HasForeignKey(s => s.CharacterSpecAbilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ActionAssets)
            .WithOne(a => a.Ability)
            .HasForeignKey(a => a.CharacterSpecAbilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CharacterSpec)
            .WithMany(s => s.Abilities)
            .HasForeignKey(e => e.CharacterSpecId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
