using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class OriginBonusEntityConfiguration : IEntityTypeConfiguration<OriginBonusEntity>
{
    public void Configure(EntityTypeBuilder<OriginBonusEntity> builder)
    {
        builder.ToTable("origin_bonuses");

        builder.HasIndex(e => e.OriginId);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.HasIndex(e => e.OriginId)
            .HasDatabaseName("IX_origin_bonuses_originId");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.OriginId)
            .HasColumnName("originId");

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.HasOne(e => e.Origin)
            .WithMany(o => o.Bonuses)
            .HasForeignKey(o => o.OriginId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_origin_bonuses_originId_origins_id");
    }
}