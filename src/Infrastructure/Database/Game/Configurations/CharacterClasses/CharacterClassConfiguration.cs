using Domain.Game.CharacterClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.CharacterClasses;

internal sealed class CharacterClassConfiguration : IEntityTypeConfiguration<CharacterClass>
{
    public void Configure(EntityTypeBuilder<CharacterClass> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(e => e.Description)
            .HasMaxLength(512);

        builder.HasMany(e => e.Stats)
            .WithOne(s => s.CharacterClass)
            .HasForeignKey(s => s.CharacterClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ActionAssets)
            .WithOne(a => a.CharacterClass)
            .HasForeignKey(a => a.CharacterClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Characters)
            .WithOne(a => a.Class)
            .HasForeignKey(a => a.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
