using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class OriginEntityConfiguration : IEntityTypeConfiguration<OriginEntity>
{
    public void Configure(EntityTypeBuilder<OriginEntity> builder)
    {
        builder.ToTable("origins");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Advantage)
            .HasColumnName("advantage");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.PlayerDescription)
            .HasColumnName("playerDescription");

        builder.Property(e => e.PlayerSummary)
            .HasColumnName("playerSummary");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Size)
            .HasColumnName("size");

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.Property(e => e.Data)
            .HasColumnName("data")
            .HasColumnType("json");
    }
}