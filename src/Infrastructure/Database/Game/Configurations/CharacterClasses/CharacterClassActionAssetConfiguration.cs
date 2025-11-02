using Domain.Game.CharacterClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.CharacterClasses;

internal sealed class CharacterClassActionAssetConfiguration : IEntityTypeConfiguration<CharacterClassActionAsset>
{
    public void Configure(EntityTypeBuilder<CharacterClassActionAsset> builder)
    {
        builder.HasKey(x => new { x.CharacterClassId, x.ActionType, x.Variant });

        builder.Property(x => x.ActionType)
            .HasConversion<int>();

        builder.OwnsOne(x => x.Animation, builder =>
        {
            builder.Property(x => x.Url).HasMaxLength(256);
            builder.OwnsOne(a => a.Scale);
        });

        builder.HasOne(x => x.CharacterClass)
            .WithMany(e => e.ActionAssets)
            .HasForeignKey(x => x.CharacterClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
