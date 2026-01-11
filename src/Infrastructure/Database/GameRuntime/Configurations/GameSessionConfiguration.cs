using Domain;
using Domain.GameRuntime.GameSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Database.GameRuntime.Configurations;

internal sealed class GameSessionConfiguration : IEntityTypeConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(c => c.ArenaId)
            .HasConversion(v => v.Value, v => new ArenaId(v))
            .IsRequired();

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(256);

        builder.OwnsOne(x => x.Result, builder =>
        {
            builder.Property(c => c.Outcome)
                .HasConversion<int>();

            var unitIdConverter = new ValueConverter<UnitId?, Guid?>(
                v => v == null ? null : v.Value.Value,
                v => v == null ? null : new UnitId(v.Value));

            builder.Property(e => e.WinnerUnitId)
                   .HasConversion(unitIdConverter)
                   .IsRequired(false);
        });
    }
}
