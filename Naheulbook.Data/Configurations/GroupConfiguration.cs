using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("group");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);

            builder.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("json");

            builder.Property(e => e.LocationId)
                .IsRequired()
                .HasColumnName("location");

            builder.Property(e => e.MasterId)
                .IsRequired()
                .HasColumnName("master");

            builder.Property(e => e.CombatLootId)
                .IsRequired(false)
                .HasColumnName("combatlootid");

            builder.HasOne(e => e.CombatLoot)
                .WithMany()
                .HasForeignKey(e => e.CombatLootId)
                .HasConstraintName("FK_group_loot_combatlootid");

            builder.HasOne(e => e.Master)
                .WithMany(e => e.Groups)
                .HasForeignKey(e => e.MasterId)
                .HasConstraintName("FK_group_user_master");

            builder.HasOne(e => e.Location)
                .WithMany()
                .HasForeignKey(e => e.LocationId)
                .HasConstraintName("FK_group_location_location");
        }
    }
}