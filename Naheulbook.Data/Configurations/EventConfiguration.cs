using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("events");

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.Description)
                .HasColumnName("description");
            builder.Property(e => e.Timestamp)
                .HasColumnName("timestamp");
            builder.Property(e => e.GroupId)
                .HasColumnName("groupid");

            builder.HasOne(e => e.Group)
                .WithMany(e => e.Events)
                .HasForeignKey(e => e.GroupId)
                .HasConstraintName("event_group_id_fk");
        }
    }
}