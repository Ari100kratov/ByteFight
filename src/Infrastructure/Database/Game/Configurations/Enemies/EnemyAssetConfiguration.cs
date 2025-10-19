using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Enemies;

internal sealed class EnemyAssetConfiguration : IEntityTypeConfiguration<EnemyAsset>
{
    public void Configure(EntityTypeBuilder<EnemyAsset> builder)
    {
        builder.HasKey(x => new { x.EnemyId, x.ActionType });

        builder.Property(x => x.ActionType)
            .HasConversion<int>();

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasOne(x => x.Enemy)
            .WithMany(e => e.Assets)
            .HasForeignKey(x => x.EnemyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
