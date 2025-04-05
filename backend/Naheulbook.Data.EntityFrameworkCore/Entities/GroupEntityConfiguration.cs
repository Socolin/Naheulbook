using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class GroupEntityConfiguration : IEntityTypeConfiguration<GroupEntity>
{
    public void Configure(EntityTypeBuilder<GroupEntity> builder)
    {
        builder.ToTable("groups");

        builder.HasIndex(e => e.MasterId)
            .HasDatabaseName("IX_groups_masterId");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Data)
            .HasColumnName("data")
            .HasColumnType("json");

        builder.Property(e => e.MasterId)
            .IsRequired()
            .HasColumnName("masterId");

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
            .HasConstraintName("FK_groups_masterId_users_id");
    }
}