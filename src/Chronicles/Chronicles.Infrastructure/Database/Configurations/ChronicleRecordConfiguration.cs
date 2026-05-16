using Chronicles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronicles.Infrastructure.Database.Configurations;

internal sealed class ChronicleRecordConfiguration : IEntityTypeConfiguration<ChronicleRecord>
{
    public void Configure(EntityTypeBuilder<ChronicleRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.NominationType, x.Value });
        builder.HasIndex(x => new { x.SessionId, x.NominationType, x.CharacterId }).IsUnique();

        builder.Property(x => x.CharacterName)
            .HasMaxLength(128);

        builder.Property(x => x.UserFirstName)
            .HasMaxLength(100);

        builder.Property(x => x.UserLastName)
            .HasMaxLength(100);

        builder.Property(x => x.CharacterClassName)
            .HasMaxLength(128);

        builder.Property(x => x.CharacterSpecName)
            .HasMaxLength(128);

        builder.Property(x => x.NominationType)
            .HasConversion<int>();

        builder.Property(x => x.Value)
            .HasPrecision(18, 4);
    }
}
