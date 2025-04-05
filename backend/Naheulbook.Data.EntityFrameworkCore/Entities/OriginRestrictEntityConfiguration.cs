using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class OriginRestrictEntityConfiguration : IEntityTypeConfiguration<OriginRestrictEntity>
{
    public void Configure(EntityTypeBuilder<OriginRestrictEntity> builder)
    {
        builder.ToTable("origin_restrictions");

        builder.HasIndex(e => e.OriginId)
            .HasDatabaseName("IX_origin_restrictions_originId");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.OriginId)
            .HasColumnName("originid");

        builder.Property(e => e.Text)
            .IsRequired()
            .HasColumnName("text");

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.HasOne(e => e.Origin)
            .WithMany(o => o.Restrictions)
            .HasForeignKey(o => o.OriginId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_origin_restrictions_originId_origins_id");
    }
}