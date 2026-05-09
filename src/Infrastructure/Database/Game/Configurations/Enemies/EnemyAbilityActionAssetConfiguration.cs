using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Enemies;

internal sealed class EnemyAbilityActionAssetConfiguration : IEntityTypeConfiguration<EnemyAbilityActionAsset>
{
    public void Configure(EntityTypeBuilder<EnemyAbilityActionAsset> builder)
    {
        builder.HasKey(x => new
        {
            x.EnemyAbilityId,
            x.ActionType,
            x.Variant
        });

        builder.Property(x => x.ActionType)
            .HasConversion<int>();

        builder.Property(x => x.Variant)
            .IsRequired();

        builder.OwnsOne(x => x.Animation, animationBuilder =>
        {
            animationBuilder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(256);

            animationBuilder.OwnsOne(x => x.Scale);
        });

        builder.HasOne(x => x.Ability)
            .WithMany(e => e.ActionAssets)
            .HasForeignKey(x => x.EnemyAbilityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
