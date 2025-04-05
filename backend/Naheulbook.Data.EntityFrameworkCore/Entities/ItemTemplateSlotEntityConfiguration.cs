using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateSlotEntityConfiguration : IEntityTypeConfiguration<ItemTemplateSlotEntity>
{
    public void Configure(EntityTypeBuilder<ItemTemplateSlotEntity> builder)
    {
        builder.HasKey(e => new {Slot = e.SlotId, Item = e.ItemTemplateId});

        builder.ToTable("item_template_slots");

        builder.HasIndex(e => e.ItemTemplateId)
            .HasDatabaseName("IX_item_template_slots_itemTemplateId");

        builder.Property(e => e.SlotId)
            .HasColumnName("slot");

        builder.Property(e => e.ItemTemplateId)
            .HasColumnName("itemTemplateId");

        builder.HasOne(s => s.ItemTemplate)
            .WithMany(s => s.Slots)
            .HasForeignKey(s => s.ItemTemplateId)
            .HasConstraintName("FK_item_template_slots_itemTemplateId_item_templates_id");

        builder.HasOne(s => s.Slot)
            .WithMany()
            .HasForeignKey(s => s.SlotId)
            .HasConstraintName("FK_item_template_slot_item_slot_slot");
    }
}