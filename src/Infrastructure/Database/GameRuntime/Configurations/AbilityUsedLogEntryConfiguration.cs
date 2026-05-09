using Domain;
using Domain.GameRuntime.GameActionLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.GameRuntime.Configurations;

internal sealed class AbilityUsedLogEntryConfiguration
    : IEntityTypeConfiguration<AbilityUsedLogEntry>
{
    public void Configure(EntityTypeBuilder<AbilityUsedLogEntry> builder)
    {
        builder.Property(x => x.AbilityType)
            .HasConversion<int>();

        builder.Property(x => x.EffectType)
            .HasConversion<int>();

        builder.Property(x => x.AbilityName)
            .HasMaxLength(128);

        builder.Property(x => x.TargetId)
            .HasConversion(v => v.Value, v => new UnitId(v))
            .IsRequired();

        builder.Property(x => x.TargetName)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.Value)
            .IsRequired();

        builder.Property(x => x.FacingDirection)
            .HasConversion<int>();

        builder.OwnsOne(x => x.TargetHp);
    }
}
