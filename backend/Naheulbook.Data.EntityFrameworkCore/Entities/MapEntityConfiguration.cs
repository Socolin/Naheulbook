using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MapEntityConfiguration : IEntityTypeConfiguration<MapEntity>
{
    public void Configure(EntityTypeBuilder<MapEntity> builder)
    {
        builder.ToTable("maps");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Data)
            .HasColumnName("data")
            .HasColumnType("json");

        builder.Property(e => e.ImageData)
            .IsRequired()
            .HasColumnName("imageData")
            .HasColumnType("json");
    }
}