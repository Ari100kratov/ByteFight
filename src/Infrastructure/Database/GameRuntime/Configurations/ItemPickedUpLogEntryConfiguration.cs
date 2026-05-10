using Domain.GameRuntime.GameActionLogs;
using Domain.GameRuntime.GameActionLogs.Entries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.GameRuntime.Configurations;

internal sealed class ItemPickedUpLogEntryConfiguration
    : IEntityTypeConfiguration<ItemPickedUpLogEntry>
{
    public void Configure(EntityTypeBuilder<ItemPickedUpLogEntry> builder)
    {
        builder.Property(x => x.PlacedItemId)
            .IsRequired();

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.ItemName)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.ItemType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();

        builder.OwnsOne(x => x.Position);

        builder.OwnsOne(x => x.ActorHp);
    }
}
