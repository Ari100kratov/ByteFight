using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Enemies;

internal sealed class EnemyActionAssetConfiguration : IEntityTypeConfiguration<EnemyActionAsset>
{
    public void Configure(EntityTypeBuilder<EnemyActionAsset> builder)
    {
        builder.HasKey(x => new { x.EnemyId, x.ActionType, x.Variant });

        builder.Property(x => x.ActionType)
            .HasConversion<int>();

        builder.OwnsOne(x => x.Animation, builder =>
        {
            builder.Property(x => x.Url).HasMaxLength(256);
            builder.OwnsOne(a => a.Scale);
        });

        builder.HasOne(x => x.Enemy)
            .WithMany(e => e.ActionAssets)
            .HasForeignKey(x => x.EnemyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
