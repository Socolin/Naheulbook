using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class GroupInviteEntityConfiguration : IEntityTypeConfiguration<GroupInviteEntity>
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