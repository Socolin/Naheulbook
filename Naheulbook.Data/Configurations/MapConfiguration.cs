using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class MapConfiguration : IEntityTypeConfiguration<Map>
    {
        public void Configure(EntityTypeBuilder<Map> builder)
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

    public class MapLayerConfiguration : IEntityTypeConfiguration<MapLayer>
    {
        public void Configure(EntityTypeBuilder<MapLayer> builder)
        {
            builder.ToTable("map_layers");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(255);

            builder.Property(e => e.Source)
                .HasColumnName("source")
                .HasMaxLength(25);

            builder.Property(e => e.IsGm)
                .HasColumnName("isGm");

            builder.HasOne(e => e.Map!)
                .WithMany(e => e.Layers)
                .HasForeignKey(e => e.MapId)
                .HasConstraintName("FK_map_layer_map");

            builder.HasOne(e => e.User!)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_map_layer_user");
        }
    }

    public class MapMarkerConfiguration : IEntityTypeConfiguration<MapMarker>
    {
        public void Configure(EntityTypeBuilder<MapMarker> builder)
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

            builder.HasOne(e => e.Layer!)
                .WithMany(e => e.Markers)
                .HasForeignKey(e => e.LayerId)
                .HasConstraintName("FK_map_marker_map_layer");
        }
    }

    public class MapMarkerLinkConfiguration : IEntityTypeConfiguration<MapMarkerLink>
    {
        public void Configure(EntityTypeBuilder<MapMarkerLink> builder)
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

            builder.HasOne(e => e.MapMarker!)
                .WithMany(e => e.Links)
                .HasForeignKey(e => e.MapMarkerId)
                .HasConstraintName("fk_map_markers_links_map_markers");

            builder.HasOne(e => e.TargetMap!)
                .WithMany()
                .HasForeignKey(e => e.TargetMapId)
                .HasConstraintName("fk_map_markers_link_target_map");

            builder.HasOne(e => e.TargetMapMarker!)
                .WithMany()
                .HasForeignKey(e => e.TargetMapMarkerId)
                .HasConstraintName("fk_map_markers_links_target_map_marker");
        }
    }
}