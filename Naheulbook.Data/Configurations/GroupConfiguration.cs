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

    public class GroupInviteConfiguration : IEntityTypeConfiguration<GroupInvite>
    {
        public void Configure(EntityTypeBuilder<GroupInvite> builder)
        {
            builder.ToTable("group_invitations");

            builder.HasKey(e => new {e.GroupId, e.CharacterId});

            builder.Property(e => e.CharacterId)
                .HasColumnName("character");

            builder.Property(e => e.GroupId)
                .HasColumnName("group");

            builder.Property(e => e.FromGroup)
                .HasColumnName("fromgroup");

            builder.HasOne(e => e.Group)
                .WithMany(g => g.Invites)
                .HasForeignKey(e => e.GroupId)
                .HasConstraintName("FK_group_invitations_group_group");

            builder.HasOne(e => e.Character)
                .WithMany(g => g.Invites)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_group_invitations_character_character");
        }
    }
}