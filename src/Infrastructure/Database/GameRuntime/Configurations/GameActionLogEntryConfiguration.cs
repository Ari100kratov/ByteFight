using Domain;
using Domain.GameRuntime.GameActionLogs;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.GameRuntime.Configurations;

internal sealed class GameActionLogEntryConfiguration
    : IEntityTypeConfiguration<GameActionLogEntry>
{
    public void Configure(EntityTypeBuilder<GameActionLogEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntryType)
            .HasConversion<int>();

        builder.Property(x => x.Info)
            .HasMaxLength(256);

        builder.Property(x => x.ActorId)
            .HasConversion(v => v.Value, v => new UnitId(v))
            .IsRequired();

        builder.Property(x => x.ActorName)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasDiscriminator(x => x.EntryType)
            .HasValue<IdleLogEntry>(GameActionLogEntryType.Idle)
            .HasValue<WalkLogEntry>(GameActionLogEntryType.Walk)
            .HasValue<AbilityUsedLogEntry>(GameActionLogEntryType.AbilityUsed)
            .HasValue<DeathLogEntry>(GameActionLogEntryType.Death)
            .HasValue<ItemPickedUpLogEntry>(GameActionLogEntryType.ItemPickedUp);

        builder.HasOne<GameSession>()
            .WithMany(s => s.ActionLogs)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
