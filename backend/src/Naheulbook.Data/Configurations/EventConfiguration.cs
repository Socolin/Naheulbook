using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.ToTable("events");

        builder.HasIndex(e => e.GroupId)
            .HasDatabaseName("IX_events_groupId");

        builder.Property(e => e.Id)
            .HasColumnName("id");
        builder.Property(e => e.Description)
            .HasColumnName("description");
        builder.Property(e => e.Timestamp)
            .HasColumnName("timestamp");
        builder.Property(e => e.GroupId)
            .HasColumnName("groupId");

        builder.HasOne(e => e.Group)
            .WithMany(e => e.Events)
            .HasForeignKey(e => e.GroupId)
            .HasConstraintName("FK_events_groupId_groups_id");
    }
}