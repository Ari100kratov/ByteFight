using Chronicles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronicles.Infrastructure.Database.Configurations;

internal sealed class ChronicleNominationConfiguration : IEntityTypeConfiguration<ChronicleNomination>
{
    public void Configure(EntityTypeBuilder<ChronicleNomination> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Type).IsUnique();

        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.Property(x => x.Code)
            .HasMaxLength(64);

        builder.Property(x => x.Title)
            .HasMaxLength(128);

        builder.Property(x => x.Description)
            .HasMaxLength(256);

        builder.Property(x => x.MetricLabel)
            .HasMaxLength(64);

        builder.Property(x => x.MetricUnit)
            .HasMaxLength(32);

        builder.Property(x => x.ValueKind)
            .HasConversion<int>();

        builder.Property(x => x.SortDirection)
            .HasConversion<int>();
    }
}
