using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("location");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("json");
            builder.Property(e => e.Name)
                .HasColumnName("name");
            builder.Property(e => e.ParentId)
                .HasColumnName("parent");

            builder.HasOne(e => e.Parent)
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .HasConstraintName("FK_location_location_parent");
        }
    }

    public class LocationMapConfiguration : IEntityTypeConfiguration<LocationMap>
    {
        public void Configure(EntityTypeBuilder<LocationMap> builder)
        {
            builder.ToTable("location_map");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("json");
            builder.Property(e => e.Name)
                .HasColumnName("name");
            builder.Property(e => e.IsGm)
                .HasColumnName("gm");
            builder.Property(e => e.LocationId)
                .HasColumnName("location");
            builder.Property(e => e.File)
                .HasColumnName("file");

            builder.HasOne(e => e.Location)
                .WithMany(e => e.Maps)
                .HasForeignKey(e => e.LocationId)
                .HasConstraintName("FK_location_map_location_location");
        }
    }
}