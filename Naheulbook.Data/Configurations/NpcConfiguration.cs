using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class NpcConfiguration : IEntityTypeConfiguration<NpcEntity>
    {
        public void Configure(EntityTypeBuilder<NpcEntity> builder)
        {
            builder.ToTable("npcs");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .HasColumnName("name");

            builder.Property(e => e.Data)
                .HasColumnType("json")
                .HasColumnName("data");

            builder.Property(e => e.GroupId)
                .HasColumnName("groupId");

            builder.HasOne(e => e.Group)
                .WithMany(g => g.Npcs)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_npcs_groupId_groups_id");
        }
    }
}