using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CalendarEntityConfiguration : IEntityTypeConfiguration<CalendarEntity>
{
    public void Configure(EntityTypeBuilder<CalendarEntity> builder)
    {
        builder.ToTable("calendars");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.StartDay)
            .HasColumnName("startday");

        builder.Property(e => e.EndDay)
            .HasColumnName("endday");

        builder.Property(e => e.Name)
            .HasColumnName("name");

        builder.Property(e => e.Note)
            .HasColumnName("note");
    }
}