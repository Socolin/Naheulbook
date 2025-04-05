using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MonsterTraitEntityConfiguration : IEntityTypeConfiguration<MonsterTraitEntity>
{
    public void Configure(EntityTypeBuilder<MonsterTraitEntity> builder)
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