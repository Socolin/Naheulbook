using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class MapConfiguration : IEntityTypeConfiguration<Map>
    {
        public void Configure(EntityTypeBuilder<Map> builder)
        {
            builder.ToTable("map");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(255);

            builder.Property(e => e.Data)
                .HasColumnName("data")
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
}