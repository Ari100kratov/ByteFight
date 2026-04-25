using Domain.Game.CharacterSpecs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.CharacterSpecs;

internal sealed class CharacterSpecActionAssetConfiguration : IEntityTypeConfiguration<CharacterSpecActionAsset>
{
    public void Configure(EntityTypeBuilder<CharacterSpecActionAsset> builder)
    {
        builder.HasKey(x => new { x.CharacterSpecId, x.ActionType, x.Variant });

        builder.Property(x => x.ActionType)
            .HasConversion<int>();

        builder.OwnsOne(x => x.Animation, builder =>
        {
            builder.Property(x => x.Url).HasMaxLength(256);
            builder.OwnsOne(a => a.Scale);
        });

        builder.HasOne(x => x.CharacterSpec)
            .WithMany(e => e.ActionAssets)
            .HasForeignKey(x => x.CharacterSpecId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
