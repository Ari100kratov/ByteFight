using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.CharacterSpecs;

internal sealed class CharacterSpecConfiguration : IEntityTypeConfiguration<CharacterSpec>
{
    public void Configure(EntityTypeBuilder<CharacterSpec> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(e => e.Description)
            .HasMaxLength(512);

        builder.HasMany(e => e.Stats)
            .WithOne(s => s.CharacterSpec)
            .HasForeignKey(s => s.CharacterSpecId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ActionAssets)
            .WithOne(a => a.CharacterSpec)
            .HasForeignKey(a => a.CharacterSpecId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Characters)
            .WithOne(a => a.Spec)
            .HasForeignKey(a => a.SpecId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
