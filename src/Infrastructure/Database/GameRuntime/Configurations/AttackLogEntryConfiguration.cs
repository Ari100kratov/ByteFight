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
    }
}
