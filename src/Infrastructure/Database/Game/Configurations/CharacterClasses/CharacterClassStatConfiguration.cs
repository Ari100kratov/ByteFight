using Domain.Game.CharacterClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.CharacterClasses;

internal sealed class CharacterClassStatConfiguration : IEntityTypeConfiguration<CharacterClassStat>
{
    public void Configure(EntityTypeBuilder<CharacterClassStat> builder)
    {
        builder.HasKey(x => new { x.CharacterClassId, x.StatType });

        builder.Property(x => x.StatType)
            .HasConversion<int>();

        builder.HasOne(x => x.CharacterClass)
            .WithMany(e => e.Stats)
            .HasForeignKey(x => x.CharacterClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
