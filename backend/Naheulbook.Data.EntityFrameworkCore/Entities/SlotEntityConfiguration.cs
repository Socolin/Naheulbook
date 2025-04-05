using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SlotEntityConfiguration : IEntityTypeConfiguration<SlotEntity>
{
    public void Configure(EntityTypeBuilder<SlotEntity> builder)
    {
        builder.ToTable("slots");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Count)
            .HasColumnName("count");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Stackable)
            .HasColumnName("stackable");

        builder.Property(e => e.TechName)
            .IsRequired()
            .HasColumnName("techname")
            .HasMaxLength(255);
    }
}