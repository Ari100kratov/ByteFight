using Domain.Game.CharacterSpecAbilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.CharacterSpecAbilities;

internal sealed class CharacterSpecAbilityStatConfiguration : IEntityTypeConfiguration<CharacterSpecAbilityStat>
{
    public void Configure(EntityTypeBuilder<CharacterSpecAbilityStat> builder)
    {
        builder.HasKey(x => new { x.CharacterSpecAbilityId, x.StatType });

        builder.Property(x => x.StatType)
            .HasConversion<int>();

        builder.Property(x => x.Value)
            .IsRequired();

        builder.HasOne(x => x.Ability)
            .WithMany(e => e.Stats)
            .HasForeignKey(x => x.CharacterSpecAbilityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
