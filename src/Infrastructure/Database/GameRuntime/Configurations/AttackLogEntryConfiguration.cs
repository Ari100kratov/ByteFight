using Domain;
using Domain.GameRuntime.GameActionLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.GameRuntime.Configurations;

internal sealed class AttackLogEntryConfiguration
    : IEntityTypeConfiguration<AttackLogEntry>
{
    public void Configure(EntityTypeBuilder<AttackLogEntry> builder)
    {
        builder.OwnsOne(x => x.TargetHp);

        builder.Property(e => e.TargetId)
            .HasConversion(v => v.Value, v => new UnitId(v))
            .IsRequired();

        builder.Property(c => c.TargetName)
            .IsRequired()
            .HasMaxLength(32);
    }
}
