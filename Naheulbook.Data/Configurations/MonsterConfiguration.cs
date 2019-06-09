using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class MonsterCategoryConfiguration : IEntityTypeConfiguration<MonsterCategory>
    {
        public void Configure(EntityTypeBuilder<MonsterCategory> builder)
        {
            builder.ToTable("monster_category");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.Name)
                .HasColumnName("name");
            builder.Property(e => e.TypeId)
                .HasColumnName("typeid");

            builder.HasOne(x => x.Type)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.TypeId)
                .HasConstraintName("monster_category_monster_type_id_fk");
        }
    }

    public class MonsterLocationConfiguration : IEntityTypeConfiguration<MonsterLocation>
    {
        public void Configure(EntityTypeBuilder<MonsterLocation> builder)
        {
            builder.ToTable("monster_location");

            builder.HasKey(e => new {e.LocationId, e.MonsterTemplateId});

            builder.Property(e => e.MonsterTemplateId)
                .HasColumnName("monsterid");
            builder.Property(e => e.LocationId)
                .HasColumnName("locationid");

            builder.HasOne(x => x.MonsterTemplate)
                .WithMany(x => x.Locations)
                .HasForeignKey(x => x.MonsterTemplateId)
                .HasConstraintName("FK_monster_location_monster_template_monsterid");

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .HasConstraintName("FK_monster_location_location_locationid");
        }
    }

    public class MonsterTemplateConfiguration : IEntityTypeConfiguration<MonsterTemplate>
    {
        public void Configure(EntityTypeBuilder<MonsterTemplate> builder)
        {
            builder.ToTable("monster_template");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("json");
            builder.Property(e => e.Name)
                .HasColumnName("name");
            builder.Property(e => e.CategoryId)
                .HasColumnName("categoryid");

            builder.HasOne(x => x.Category)
                .WithMany(x => x.MonsterTemplates)
                .HasForeignKey(x => x.CategoryId)
                .HasConstraintName("FK_monster_template_monster_type_type");
        }
    }

    public class MonsterTemplateSimpleInventoryConfiguration : IEntityTypeConfiguration<MonsterTemplateSimpleInventory>
    {
        public void Configure(EntityTypeBuilder<MonsterTemplateSimpleInventory> builder)
        {
            builder.ToTable("monster_template_simple_inventory");

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
            builder.ToTable("monster_type");

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
            builder.ToTable("monster_trait");

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