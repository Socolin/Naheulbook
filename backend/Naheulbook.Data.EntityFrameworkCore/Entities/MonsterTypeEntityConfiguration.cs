using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MonsterTypeEntityConfiguration : IEntityTypeConfiguration<MonsterTypeEntity>
{
    public void Configure(EntityTypeBuilder<MonsterTypeEntity> builder)
    {
        builder.ToTable("monster_types");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");
        builder.Property(e => e.Name)
            .HasColumnName("name");
    }
}