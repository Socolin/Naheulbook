using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MapLayerEntityConfiguration : IEntityTypeConfiguration<MapLayerEntity>
{
    public void Configure(EntityTypeBuilder<MapLayerEntity> builder)
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

        builder.HasOne(e => e.Map)
            .WithMany(e => e.Layers)
            .HasForeignKey(e => e.MapId)
            .HasConstraintName("FK_map_layer_map");

        builder.HasOne(e => e.User!)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_map_layer_user");
    }
}