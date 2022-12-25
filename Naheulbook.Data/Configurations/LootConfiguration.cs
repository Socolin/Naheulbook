using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class LootConfiguration : IEntityTypeConfiguration<LootEntity>
    {
        public void Configure(EntityTypeBuilder<LootEntity> builder)
        {
            builder.ToTable("loots");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);

            builder.Property(e => e.Created)
                .IsRequired(false)
                .HasColumnName("dead");

            builder.Property(e => e.GroupId)
                .IsRequired()
                .HasColumnName("groupid");

            builder.Property(e => e.IsVisibleForPlayer)
                .HasColumnName("visibleForPlayer");

            builder.HasOne(e => e.Group)
                .WithMany(e => e.Loots)
                .HasForeignKey(e => e.GroupId)
                .HasConstraintName("FK_loot_group_groupid");
        }
    }
}