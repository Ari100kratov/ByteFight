using Domain.Game.CharacterSpecAbilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.CharacterSpecAbilities;

internal sealed class CharacterSpecAbilityActionAssetConfiguration : IEntityTypeConfiguration<CharacterSpecAbilityActionAsset>
{
    public void Configure(EntityTypeBuilder<CharacterSpecAbilityActionAsset> builder)
    {
        builder.HasKey(x => new
        {
            x.CharacterSpecAbilityId,
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
            .HasForeignKey(x => x.CharacterSpecAbilityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
