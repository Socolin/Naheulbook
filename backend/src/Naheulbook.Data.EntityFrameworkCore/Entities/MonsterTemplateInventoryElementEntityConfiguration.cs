using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MonsterTemplateInventoryElementEntityConfiguration : IEntityTypeConfiguration<MonsterTemplateInventoryElementEntity>
{
    public void Configure(EntityTypeBuilder<MonsterTemplateInventoryElementEntity> builder)
    {
        builder.ToTable("monster_template_inventory_elements");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");
        builder.Property(e => e.Chance)
            .HasColumnName("chance");
        builder.Property(e => e.MaxCount)
            .HasColumnName("maxCount");
        builder.Property(e => e.MinCount)
            .HasColumnName("minCount");
        builder.Property(e => e.ItemTemplateId)
            .HasColumnName("itemTemplateId");
        builder.Property(e => e.MonsterTemplateId)
            .HasColumnName("monstertemplateid");

        builder.HasOne(x => x.ItemTemplate)
            .WithMany()
            .HasForeignKey(x => x.ItemTemplateId)
            .HasConstraintName("FK_monster_template_inventory_elements_itemTemplateId");

        builder.HasOne(x => x.MonsterTemplate)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.MonsterTemplateId)
            .HasConstraintName("FK_monster_template_simple_inventory_monster_template_monstertem");
    }
}