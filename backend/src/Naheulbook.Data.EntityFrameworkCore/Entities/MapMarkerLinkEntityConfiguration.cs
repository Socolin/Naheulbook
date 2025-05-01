using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MapMarkerLinkEntityConfiguration : IEntityTypeConfiguration<MapMarkerLinkEntity>
{
    public void Configure(EntityTypeBuilder<MapMarkerLinkEntity> builder)
    {
        builder.ToTable("map_marker_links");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .IsRequired(false)
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.MapMarkerId)
            .HasColumnName("mapMarkerId");

        builder.Property(e => e.TargetMapId)
            .HasColumnName("targetMapId");

        builder.Property(e => e.TargetMapMarkerId)
            .IsRequired(false)
            .HasColumnName("targetMapMarkerId");

        builder.HasOne(e => e.MapMarker)
            .WithMany(e => e.Links)
            .HasForeignKey(e => e.MapMarkerId)
            .HasConstraintName("fk_map_markers_links_map_markers");

        builder.HasOne(e => e.TargetMap)
            .WithMany()
            .HasForeignKey(e => e.TargetMapId)
            .HasConstraintName("fk_map_markers_link_target_map");

        builder.HasOne(e => e.TargetMapMarker!)
            .WithMany()
            .HasForeignKey(e => e.TargetMapMarkerId)
            .HasConstraintName("fk_map_markers_links_target_map_marker");
    }
}