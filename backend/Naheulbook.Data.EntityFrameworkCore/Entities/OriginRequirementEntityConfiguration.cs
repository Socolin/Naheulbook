using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class OriginRequirementEntityConfiguration : IEntityTypeConfiguration<OriginRequirementEntity>
{
    public void Configure(EntityTypeBuilder<OriginRequirementEntity> builder)
    {
        builder.ToTable("origin_requirements");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.MaxValue)
            .HasColumnName("maxvalue");

        builder.Property(e => e.MinValue)
            .HasColumnName("minvalue");

        builder.Property(e => e.OriginId)
            .IsRequired()
            .HasColumnName("originid");

        builder.Property(e => e.StatName)
            .IsRequired()
            .HasColumnName("stat")
            .HasMaxLength(64);

        builder.HasIndex(e => e.OriginId)
            .HasDatabaseName("IX_origin_requirements_originId");

        builder.HasIndex(e => e.StatName)
            .HasDatabaseName("IX_origin_requirement_stat");

        builder.HasOne(e => e.Origin)
            .WithMany(o => o.Requirements)
            .HasForeignKey(e => e.OriginId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_origin_requirements_originId_origins_id");
    }
}