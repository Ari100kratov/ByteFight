using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.CharacterSpecs;

internal sealed class CharacterSpecStatConfiguration : IEntityTypeConfiguration<CharacterSpecStat>
{
    public void Configure(EntityTypeBuilder<CharacterSpecStat> builder)
    {
        builder.HasKey(x => new { x.CharacterSpecId, x.StatType });

        builder.Property(x => x.StatType)
            .HasConversion<int>();

        builder.HasOne(x => x.CharacterSpec)
            .WithMany(e => e.Stats)
            .HasForeignKey(x => x.CharacterSpecId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
