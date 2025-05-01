using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MapMarkerEntityConfiguration : IEntityTypeConfiguration<MapMarkerEntity>
{
    public void Configure(EntityTypeBuilder<MapMarkerEntity> builder)
    {
        builder.ToTable("map_markers");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Type)
            .HasColumnName("type")
            .HasMaxLength(25);

        builder.Property(e => e.Description)
            .IsRequired(false)
            .HasColumnName("description")
            .HasMaxLength(10000);

        builder.Property(e => e.MarkerInfo)
            .HasColumnName("markerInfo")
            .HasColumnType("json");

        builder.Property(e => e.LayerId)
            .HasColumnName("layerId");

        builder.HasOne(e => e.Layer)
            .WithMany(e => e.Markers)
            .HasForeignKey(e => e.LayerId)
            .HasConstraintName("FK_map_marker_map_layer");
    }
}