using Domain.Game.Enemies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Game.Configurations.Enemies;

internal sealed class EnemyConfiguration : IEntityTypeConfiguration<Enemy>
{
    public void Configure(EntityTypeBuilder<Enemy> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(e => e.Description)
            .HasMaxLength(512);

        builder.HasMany(e => e.Stats)
            .WithOne(s => s.Enemy)
            .HasForeignKey(s => s.EnemyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Assets)
            .WithOne(a => a.Enemy)
            .HasForeignKey(a => a.EnemyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
