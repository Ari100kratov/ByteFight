using Domain;
using Domain.GameRuntime.GameActionLogs.Entries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.GameRuntime.Configurations;

internal sealed class StatusAppliedLogEntryConfiguration
    : IEntityTypeConfiguration<StatusAppliedLogEntry>
{
    public void Configure(EntityTypeBuilder<StatusAppliedLogEntry> builder)
    {
        builder.Property(x => x.StatusType)
            .HasConversion<int>();

        builder.Property(x => x.TargetId)
            .HasConversion(v => v.Value, v => new UnitId(v))
            .IsRequired();

        builder.Property(x => x.TargetName)
            .IsRequired()
            .HasMaxLength(32);

        builder.OwnsOne(x => x.TargetHp);
    }
}
