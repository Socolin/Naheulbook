using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");

            builder.HasIndex(e => e.Username)
                .IsUnique();

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Admin)
                .HasColumnName("admin")
                .HasDefaultValueSql("false");

            builder.Property(e => e.DisplayName)
                .HasColumnName("displayname")
                .HasMaxLength(255);

            builder.Property(e => e.FbId)
                .HasColumnName("fbid")
                .HasMaxLength(255);

            builder.Property(e => e.GoogleId)
                .HasColumnName("googleid")
                .HasMaxLength(255);

            builder.Property(e => e.LiveId)
                .HasColumnName("liveid")
                .HasMaxLength(255);

            builder.Property(e => e.TwitterId)
                .HasColumnName("twitterid")
                .HasMaxLength(255);

            builder.Property(e => e.HashedPassword)
                .HasColumnName("password")
                .HasMaxLength(255);

            builder.Property(e => e.ActivationCode)
                .HasColumnName("activationCode")
                .HasMaxLength(255);

            builder.Property(e => e.Username)
                .IsRequired()
                .HasColumnName("username")
                .HasMaxLength(255);
        }
    }
}