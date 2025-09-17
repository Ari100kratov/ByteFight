using Domain;
using Domain.Game.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations;

internal sealed class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.UserId)
            .HasConversion(v => v.Value, v => new UserId(v))
            .IsRequired();
    }
}
