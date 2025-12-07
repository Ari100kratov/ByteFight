using Domain;
using Domain.GameRuntime.GameSessionParticipants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Database.GameRuntime.Configurations;

internal sealed class GameSessionParticipantConfiguration : IEntityTypeConfiguration<GameSessionParticipant>
{
    public void Configure(EntityTypeBuilder<GameSessionParticipant> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.UnitId)
            .HasConversion(v => v.Value, v => new UnitId(v))
            .IsRequired();

        var userIdConverter = new ValueConverter<UserId?, Guid?>(
            v => v == null ? null : v.Value.Value,
            v => v == null ? null : new UserId(v.Value));

        builder.Property(e => e.UserId)
               .HasConversion(userIdConverter)
               .IsRequired(false);

        builder.HasOne(e => e.Session)
            .WithMany(s => s.Participants)
            .HasForeignKey(e => e.SessionId);
    }
}
