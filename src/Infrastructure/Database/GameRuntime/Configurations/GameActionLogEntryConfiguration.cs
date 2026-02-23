using Domain.Game.Actions;
using Domain.GameRuntime.GameActionLogs;
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

        builder.Property(c => c.Info)
            .HasMaxLength(256);

        builder.HasDiscriminator(x => x.ActionType)
            .HasValue<AttackLogEntry>(ActionType.Attack)
            .HasValue<WalkLogEntry>(ActionType.Walk)
            .HasValue<DeathLogEntry>(ActionType.Dead)
            .HasValue<IdleLogEntry>(ActionType.Idle);

        builder.HasOne<GameSession>()
            .WithMany(s => s.ActionLogs)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
