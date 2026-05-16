using Chronicles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronicles.Infrastructure.Database.Configurations;

internal sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasMaxLength(256);

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb");

        builder.HasIndex(x => new { x.ProcessedAtUtc, x.DeadLetteredAtUtc, x.NextAttemptAtUtc, x.ReceivedAtUtc })
            .HasDatabaseName("ix_inbox_messages_processing_queue");

        builder.Property(x => x.Error)
            .HasMaxLength(2048);
    }
}
