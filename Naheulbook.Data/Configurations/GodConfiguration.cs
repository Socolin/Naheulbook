using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class GodConfiguration : IEntityTypeConfiguration<God>
    {
        public void Configure(EntityTypeBuilder<God> builder)
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
}