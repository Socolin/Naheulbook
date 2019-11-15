using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class MonsterSubCategoryConfiguration : IEntityTypeConfiguration<MonsterSubCategory>
    {
        public void Configure(EntityTypeBuilder<MonsterSubCategory> builder)
        {
            builder.ToTable("monster_subcategories");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.Name)
                .HasColumnName("name");
            builder.Property(e => e.TypeId)
                .HasColumnName("typeid");

            builder.HasOne(x => x.Type)
                .WithMany(x => x.SubCategories)
                .HasForeignKey(x => x.TypeId)
                .HasConstraintName("monster_category_monster_type_id_fk");
        }
    }

    public class MonsterTemplateConfiguration : IEntityTypeConfiguration<MonsterTemplate>
    {
        public void Configure(EntityTypeBuilder<MonsterTemplate> builder)
        {
            builder.ToTable("monster_templates");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("json");
            builder.Property(e => e.Name)
                .HasColumnName("name");
            builder.Property(e => e.SubCategoryId)
                .HasColumnName("subCategoryId");

            builder.HasOne(x => x.SubCategory)
                .WithMany(x => x.MonsterTemplates)
                .HasForeignKey(x => x.SubCategoryId)
                .HasConstraintName("FK_monster_template_monster_category_categoryid");
        }
    }

    public class MonsterTemplateInventoryElementConfiguration : IEntityTypeConfiguration<MonsterTemplateInventoryElement>
    {
        public void Configure(EntityTypeBuilder<MonsterTemplateInventoryElement> builder)
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
                .HasColumnName("itemtemplateid");
            builder.Property(e => e.MonsterTemplateId)
                .HasColumnName("monstertemplateid");

            builder.HasOne(x => x.ItemTemplate)
                .WithMany()
                .HasForeignKey(x => x.ItemTemplateId)
                .HasConstraintName("FK_monster_template_simple_inventory_item_template_itemtemplatei");

            builder.HasOne(x => x.MonsterTemplate)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.MonsterTemplateId)
                .HasConstraintName("FK_monster_template_simple_inventory_monster_template_monstertem");
        }
    }

    public class MonsterTypeConfiguration : IEntityTypeConfiguration<MonsterType>
    {
        public void Configure(EntityTypeBuilder<MonsterType> builder)
        {
            builder.ToTable("monster_types");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.Name)
                .HasColumnName("name");
        }
    }

    public class MonsterTraitConfiguration : IEntityTypeConfiguration<MonsterTrait>
    {
        public void Configure(EntityTypeBuilder<MonsterTrait> builder)
        {
            builder.ToTable("monster_traits");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.Name)
                .HasColumnName("name");
            builder.Property(e => e.Description)
                .HasColumnName("description");
            builder.Property(e => e.Levels)
                .HasColumnName("levels");
        }
    }
}