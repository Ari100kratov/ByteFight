using Domain.Game.Characters;
using Domain.Game.Characters.CharacterCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Characters;

internal sealed class CharacterCodeConfiguration : IEntityTypeConfiguration<CharacterCode>
{
    public void Configure(EntityTypeBuilder<CharacterCode> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(c => c.Language)
            .HasConversion<int>();

        builder.HasOne<Character>()
            .WithMany(c => c.Codes)
            .HasForeignKey(c => c.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
