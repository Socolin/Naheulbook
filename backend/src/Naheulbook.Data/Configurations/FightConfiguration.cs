using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations;

public class FightConfiguration : IEntityTypeConfiguration<FightEntity>
{
    public void Configure(EntityTypeBuilder<FightEntity> builder)
    {
        builder.ToTable("fights");

        builder.HasIndex(e => e.GroupId)
            .HasDatabaseName("IX_fights_groupId");

        builder.Property(e => e.Id)
            .HasColumnName("id");
        builder.Property(e => e.GroupId)
            .HasColumnName("groupId");

        builder.HasOne(e => e.Group)
            .WithMany(e => e.Fights)
            .HasForeignKey(e => e.GroupId)
            .HasConstraintName("FK_fights_groupId_groups_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}