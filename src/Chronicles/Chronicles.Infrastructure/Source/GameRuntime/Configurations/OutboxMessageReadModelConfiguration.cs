using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronicles.Infrastructure.Source.GameRuntime.Configurations;

internal sealed class OutboxMessageReadModelConfiguration : IEntityTypeConfiguration<OutboxMessageReadModel>
{
    public void Configure(EntityTypeBuilder<OutboxMessageReadModel> builder)
    {
        builder.ToTable("outbox_messages", global::Chronicles.Infrastructure.Database.Schemas.Integration);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.Type, x.CreatedAtUtc, x.Id });

        builder.Property(x => x.Type)
            .HasMaxLength(256);

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb");

        builder.Property(x => x.Error)
            .HasMaxLength(2048);
    }
}
