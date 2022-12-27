using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<GroupEntity>
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

public class GroupInviteConfiguration : IEntityTypeConfiguration<GroupInviteEntity>
{
    public void Configure(EntityTypeBuilder<GroupInviteEntity> builder)
    {
        builder.ToTable("group_invitations");

        builder.HasKey(e => new {e.GroupId, e.CharacterId});

        builder.HasIndex(e => e.CharacterId)
            .HasDatabaseName("IX_group_invitations_characterId");
        builder.HasIndex(e => e.GroupId)
            .HasDatabaseName("IX_group_invitations_groupId");

        builder.Property(e => e.CharacterId)
            .HasColumnName("characterId");

        builder.Property(e => e.GroupId)
            .HasColumnName("groupId");

        builder.Property(e => e.FromGroup)
            .HasColumnName("fromgroup");

        builder.HasOne(e => e.Group)
            .WithMany(g => g.Invites)
            .HasForeignKey(e => e.GroupId)
            .HasConstraintName("FK_group_invitations_groupId_groups_id");

        builder.HasOne(e => e.Character)
            .WithMany(g => g.Invites)
            .HasForeignKey(e => e.CharacterId)
            .HasConstraintName("FK_group_invitations_characterId_characters_id");
    }
}

public class GroupHistoryEntryConfiguration : IEntityTypeConfiguration<GroupHistoryEntryEntity>
{
    public void Configure(EntityTypeBuilder<GroupHistoryEntryEntity> builder)
    {
        builder.ToTable("group_history_entries");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");
        builder.Property(e => e.GroupId)
            .HasColumnName("group");
        builder.Property(e => e.Data)
            .HasColumnName("data")
            .HasColumnType("json");
        builder.Property(e => e.Date)
            .HasColumnName("date");
        builder.Property(e => e.Info)
            .HasColumnName("info");
        builder.Property(e => e.Action)
            .HasColumnName("action");
        builder.Property(e => e.Gm)
            .HasColumnName("gm");

        builder.HasOne(e => e.Group)
            .WithMany(g => g.HistoryEntries)
            .HasForeignKey(e => e.GroupId)
            .HasConstraintName("FK_group_history_group_group");
    }
}