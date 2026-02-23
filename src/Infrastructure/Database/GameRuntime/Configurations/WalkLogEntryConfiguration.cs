using Domain.GameRuntime.GameActionLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.GameRuntime.Configurations;

internal sealed class WalkLogEntryConfiguration
    : IEntityTypeConfiguration<WalkLogEntry>
{
    public void Configure(EntityTypeBuilder<WalkLogEntry> builder)
    {
        builder.OwnsOne(x => x.To);
    }
}
