using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class StatConfiguration : IEntityTypeConfiguration<Stat>
    {
        public void Configure(EntityTypeBuilder<Stat> builder)
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
}