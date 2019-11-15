using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasIndex(e => e.Username)
                .IsUnique();

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Admin)
                .HasColumnName("admin")
                .HasDefaultValueSql("false");

            builder.Property(e => e.DisplayName)
                .IsRequired(false)
                .HasColumnName("displayname")
                .HasMaxLength(255);

            builder.Property(e => e.FbId)
                .IsRequired(false)
                .HasColumnName("fbid")
                .HasMaxLength(255);

            builder.Property(e => e.GoogleId)
                .IsRequired(false)
                .HasColumnName("googleid")
                .HasMaxLength(255);

            builder.Property(e => e.MicrosoftId)
                .IsRequired(false)
                .HasColumnName("liveid")
                .HasMaxLength(255);

            builder.Property(e => e.TwitterId)
                .IsRequired(false)
                .HasColumnName("twitterid")
                .HasMaxLength(255);

            builder.Property(e => e.HashedPassword)
                .IsRequired(false)
                .HasColumnName("password")
                .HasMaxLength(255);

            builder.Property(e => e.ActivationCode)
                .IsRequired(false)
                .HasColumnName("activationCode")
                .HasMaxLength(255);

            builder.Property(e => e.Username)
                .IsRequired(false)
                .HasColumnName("username")
                .HasMaxLength(255);
        }
    }
}