using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class JobEntityConfiguration : IEntityTypeConfiguration<JobEntity>
{
    public void Configure(EntityTypeBuilder<JobEntity> builder)
    {
        builder.ToTable("jobs");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.PlayerDescription)
            .IsRequired()
            .HasColumnName("playerDescription");

        builder.Property(e => e.PlayerSummary)
            .IsRequired()
            .HasColumnName("playerSummary");

        builder.Property(e => e.Information)
            .HasColumnName("informations");

        builder.Property(e => e.IsMagic)
            .HasColumnName("ismagic")
            .HasDefaultValueSql("false");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.Property(e => e.Data)
            .HasColumnName("data")
            .HasColumnType("json");
    }
}