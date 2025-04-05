using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class StatEntityConfiguration : IEntityTypeConfiguration<StatEntity>
{
    public void Configure(EntityTypeBuilder<StatEntity> builder)
    {
        builder.HasKey(e => e.Name);

        builder.ToTable("stats");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Bonus)
            .HasColumnName("bonus");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.DisplayName)
            .IsRequired()
            .HasColumnName("displayname")
            .HasMaxLength(255);

        builder.Property(e => e.Penalty)
            .HasColumnName("penality");
    }
}