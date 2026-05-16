using Domain.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.GameRuntime.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", global::Infrastructure.Database.Schemas.Integration);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.AggregateId, x.Type })
            .IsUnique();

        builder.HasIndex(x => new { x.Type, x.CreatedAtUtc, x.Id });

        builder.Property(x => x.Type)
            .HasMaxLength(256);

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb");

        builder.Property(x => x.Error)
            .HasMaxLength(2048);
    }
}
