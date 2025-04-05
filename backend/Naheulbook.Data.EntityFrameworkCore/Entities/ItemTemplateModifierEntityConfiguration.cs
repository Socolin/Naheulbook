using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateModifierEntityConfiguration : IEntityTypeConfiguration<ItemTemplateModifierEntity>
{
    public void Configure(EntityTypeBuilder<ItemTemplateModifierEntity> builder)
    {
        builder.ToTable("item_template_modifiers");

        builder.HasIndex(e => e.ItemTemplateId)
            .HasDatabaseName("IX_item_template_modifiers_itemTemplateId");

        builder.HasIndex(e => e.RequiredJobId)
            .HasDatabaseName("IX_item_template_modifiers_requiredJobId");

        builder.HasIndex(e => e.RequiredOriginId)
            .HasDatabaseName("IX_item_template_modifiers_requiredOriginId");

        builder.HasIndex(e => e.StatName)
            .HasDatabaseName("IX_item_effect_stat");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.ItemTemplateId)
            .IsRequired()
            .HasColumnName("itemTemplateId");

        builder.Property(e => e.RequiredJobId)
            .HasColumnName("requiredJobId");

        builder.Property(e => e.RequiredOriginId)
            .HasColumnName("requiredOriginId");

        builder.Property(e => e.Special)
            .IsRequired(false)
            .HasColumnName("special")
            .HasMaxLength(2048);

        builder.Property(e => e.StatName)
            .IsRequired()
            .HasColumnName("stat")
            .HasMaxLength(64);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasColumnName("type")
            .HasMaxLength(64)
            .HasDefaultValueSql("'ADD'");

        builder.Property(e => e.Value)
            .HasColumnName("value");

        builder.HasOne(s => s.ItemTemplate)
            .WithMany(e => e.Modifiers)
            .HasForeignKey(s => s.ItemTemplateId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_item_template_modifiers_itemTemplateId_item_templates_id");

        builder.HasOne(s => s.RequiredOrigin)
            .WithMany()
            .HasForeignKey(s => s.RequiredOriginId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_item_template_modifiers_requiredOriginId_origins_id");

        builder.HasOne(s => s.RequiredJob)
            .WithMany()
            .HasForeignKey(s => s.RequiredJobId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_item_template_modifiers_requiredJobId_jobs_id");
    }
}