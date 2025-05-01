using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class GodEntityConfiguration : IEntityTypeConfiguration<GodEntity>
{
    public void Configure(EntityTypeBuilder<GodEntity> builder)
    {
        builder.ToTable("gods");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");
        builder.Property(e => e.Description)
            .HasColumnName("description");
        builder.Property(e => e.DisplayName)
            .HasColumnName("displayname");
        builder.Property(e => e.TechName)
            .HasColumnName("techname");

        builder.HasIndex(e => e.DisplayName)
            .IsUnique()
            .HasDatabaseName("god_displayname_uindex");
        builder.HasIndex(e => e.TechName)
            .IsUnique()
            .HasDatabaseName("god_techname_uindex");
    }
}