using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class OriginRandomNameUrlEntityConfiguration : IEntityTypeConfiguration<OriginRandomNameUrlEntity>
{
    public void Configure(EntityTypeBuilder<OriginRandomNameUrlEntity> builder)
    {
        builder.ToTable("origin_random_name_urls");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.OriginId)
            .HasDatabaseName("IX_origin_random_name_urls_originId");

        builder.Property(x => x.Sex)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("sex");

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("url");

        builder.Property(x => x.OriginId)
            .HasColumnName("originId");

        builder.HasOne(x => x.Origin)
            .WithMany()
            .HasForeignKey(x => x.OriginId);

        builder.HasOne(e => e.Origin)
            .WithMany()
            .HasForeignKey(e => e.OriginId)
            .HasConstraintName("FK_origin_random_name_urls_originId_origins_id")
            .OnDelete(DeleteBehavior.Cascade);

    }
}