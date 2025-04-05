using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class GroupHistoryEntryEntityConfiguration : IEntityTypeConfiguration<GroupHistoryEntryEntity>
{
    public void Configure(EntityTypeBuilder<GroupHistoryEntryEntity> builder)
    {
        builder.ToTable("group_history_entries");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");
        builder.Property(e => e.GroupId)
            .HasColumnName("group");
        builder.Property(e => e.Data)
            .HasColumnName("data")
            .HasColumnType("json");
        builder.Property(e => e.Date)
            .HasColumnName("date");
        builder.Property(e => e.Info)
            .HasColumnName("info");
        builder.Property(e => e.Action)
            .HasColumnName("action");
        builder.Property(e => e.Gm)
            .HasColumnName("gm");

        builder.HasOne(e => e.Group)
            .WithMany(g => g.HistoryEntries)
            .HasForeignKey(e => e.GroupId)
            .HasConstraintName("FK_group_history_group_group");
    }
}