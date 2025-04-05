using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateRequirementEntityConfiguration : IEntityTypeConfiguration<ItemTemplateRequirementEntity>
{
    public void Configure(EntityTypeBuilder<ItemTemplateRequirementEntity> builder)
    {
        builder.ToTable("item_template_requirements");

        builder.HasIndex(e => e.ItemTemplateId);

        builder.HasIndex(e => e.StatName);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.ItemTemplateId)
            .HasColumnName("itemTemplateId");

        builder.Property(e => e.MaxValue)
            .HasColumnName("maxvalue");

        builder.Property(e => e.MinValue)
            .HasColumnName("minvalue");

        builder.Property(e => e.StatName)
            .IsRequired()
            .HasColumnName("stat")
            .HasMaxLength(64);

        builder.HasOne(e => e.ItemTemplate)
            .WithMany(i => i.Requirements)
            .HasForeignKey(r => r.ItemTemplateId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_item_template_requirements_itemTemplateId_item_templates_id");

        builder.HasOne(e => e.Stat)
            .WithMany()
            .HasForeignKey(r => r.StatName)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_item_requirement_stat_stat");
    }
}