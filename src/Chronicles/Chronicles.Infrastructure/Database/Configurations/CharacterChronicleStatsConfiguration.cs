using Chronicles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronicles.Infrastructure.Database.Configurations;

internal sealed class CharacterChronicleStatsConfiguration : IEntityTypeConfiguration<CharacterChronicleStats>
{
    public void Configure(EntityTypeBuilder<CharacterChronicleStats> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.CharacterId).IsUnique();

        builder.Property(x => x.CharacterName)
            .HasMaxLength(128);
    }
}
