using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class OriginInfoEntityConfiguration : IEntityTypeConfiguration<OriginInfoEntity>
{
    public void Configure(EntityTypeBuilder<OriginInfoEntity> builder)
    {
        builder.ToTable("origin_information");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.OriginId)
            .HasColumnName("originId");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasColumnName("title")
            .HasMaxLength(255);

        builder.HasIndex(e => e.OriginId)
            .HasDatabaseName("IX_origin_information_originId");

        builder.HasOne(e => e.Origin)
            .WithMany(o => o.Information)
            .HasForeignKey(e => e.OriginId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_origin_information_originId_origins_id");
    }
}