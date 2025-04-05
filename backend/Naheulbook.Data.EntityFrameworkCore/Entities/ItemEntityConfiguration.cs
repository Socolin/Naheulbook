using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemEntityConfiguration : IEntityTypeConfiguration<ItemEntity>
{
    public void Configure(EntityTypeBuilder<ItemEntity> builder)
    {
        builder.ToTable("items");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Data)
            .HasColumnName("data");
        builder.Property(e => e.Modifiers)
            .HasColumnName("modifiers");
        builder.Property(e => e.ContainerId)
            .IsRequired(false)
            .HasColumnName("container");

        builder.Property(e => e.ItemTemplateId)
            .HasColumnName("itemTemplateId");

        builder.Property(e => e.CharacterId)
            .IsRequired(false)
            .HasColumnName("characterid");
        builder.Property(e => e.MonsterId)
            .IsRequired(false)
            .HasColumnName("monsterid");
        builder.Property(e => e.LootId)
            .IsRequired(false)
            .HasColumnName("lootid");

        builder.Property(e => e.LifetimeType)
            .HasColumnName("lifetimetype")
            .HasMaxLength(30)
            .HasComputedColumnSql("json_unquote(json_extract(`data`,'$.lifetime.durationType'))");

        builder.HasOne(e => e.ItemTemplate)
            .WithMany()
            .HasForeignKey(e => e.ItemTemplateId)
            .HasConstraintName("FK_items_itemTemplateId_item_templates_id");

        builder.HasOne(e => e.Container)
            .WithMany()
            .HasForeignKey(e => e.ContainerId);

        builder.HasOne(e => e.Character!)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.CharacterId)
            .HasConstraintName("FK_item_character_characterid");

        builder.HasOne(e => e.Loot!)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.LootId)
            .HasConstraintName("FK_item_loot_lootid");

        builder.HasOne(e => e.Monster!)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.MonsterId)
            .HasConstraintName("FK_item_monster_monsterid");
    }
}