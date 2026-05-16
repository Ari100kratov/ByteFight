using Chronicles.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronicles.Infrastructure.Database.Configurations;

internal sealed class OutboxImportCursorConfiguration : IEntityTypeConfiguration<OutboxImportCursor>
{
    public void Configure(EntityTypeBuilder<OutboxImportCursor> builder)
    {
        builder.HasKey(x => x.Name);

        builder.Property(x => x.Name)
            .HasMaxLength(128);
    }
}
