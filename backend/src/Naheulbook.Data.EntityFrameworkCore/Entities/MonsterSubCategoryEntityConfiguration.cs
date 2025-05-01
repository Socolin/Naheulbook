using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MonsterSubCategoryEntityConfiguration : IEntityTypeConfiguration<MonsterSubCategoryEntity>
{
    public void Configure(EntityTypeBuilder<MonsterSubCategoryEntity> builder)
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