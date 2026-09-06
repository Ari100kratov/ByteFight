using Domain.Game.Characters.CharacterTalents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Characters;

internal sealed class CharacterTalentConfiguration
    : IEntityTypeConfiguration<CharacterTalent>
{
    public void Configure(EntityTypeBuilder<CharacterTalent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TalentId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Rank)
            .HasDefaultValue(1);

        builder.HasIndex(x => new { x.CharacterId, x.TalentId })
            .IsUnique();

        builder.HasOne(x => x.Character)
            .WithMany(c => c.Talents)
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
