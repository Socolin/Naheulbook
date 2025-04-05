using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SkillEffectEntityConfiguration : IEntityTypeConfiguration<SkillEffectEntity>
{
    public void Configure(EntityTypeBuilder<SkillEffectEntity> builder)
    {
        builder.ToTable("skill_effects");

        builder.HasKey(e => new {e.SkillId, e.StatName});

        builder.HasIndex(e => e.SkillId)
            .HasDatabaseName("IX_skill_effects_skillId");

        builder.HasIndex(e => e.StatName);

        builder.Property(e => e.SkillId)
            .IsRequired()
            .HasColumnName("skillId");

        builder.Property(e => e.Value)
            .IsRequired()
            .HasColumnName("value");

        builder.Property(e => e.StatName)
            .IsRequired()
            .HasColumnName("stat")
            .HasMaxLength(255);
    }
}