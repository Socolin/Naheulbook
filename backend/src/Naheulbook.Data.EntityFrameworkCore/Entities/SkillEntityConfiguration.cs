using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SkillEntityConfiguration : IEntityTypeConfiguration<SkillEntity>
{
    public void Configure(EntityTypeBuilder<SkillEntity> builder)
    {
        builder.ToTable("skills");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Description)
            .HasColumnName("description");

        builder.Property(e => e.PlayerDescription)
            .HasColumnName("playerDescription");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Require)
            .HasColumnName("require");

        builder.Property(e => e.Resist)
            .HasColumnName("resist");

        builder.Property(e => e.Roleplay)
            .HasColumnName("roleplay");

        builder.Property(e => e.Stat)
            .HasColumnName("stat")
            .HasMaxLength(2048);

        builder.Property(e => e.Test)
            .HasColumnName("test");

        builder.Property(e => e.Using)
            .HasColumnName("using");

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.HasMany(e => e.SkillEffects)
            .WithOne(s => s.Skill)
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_skill_effects_skillId_skills_id");
    }
}