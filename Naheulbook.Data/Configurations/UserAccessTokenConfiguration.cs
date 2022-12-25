using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class UserAccessTokenConfiguration : IEntityTypeConfiguration<UserAccessTokenEntity>
    {
        public void Configure(EntityTypeBuilder<UserAccessTokenEntity> builder)
        {
            builder.ToTable("user_access_token");

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.HasIndex(e => e.Key);

            builder.Property(e => e.DateCreated)
                .HasColumnName("dateCreated");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);

            builder.Property(e => e.Key)
                .IsRequired()
                .HasColumnName("key")
                .HasMaxLength(255);

            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_user_access_token_users");
        }
    }
}