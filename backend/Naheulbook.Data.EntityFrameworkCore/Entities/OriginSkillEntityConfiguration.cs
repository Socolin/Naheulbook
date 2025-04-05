using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class OriginSkillEntityConfiguration : IEntityTypeConfiguration<OriginSkillEntity>
{
    public void Configure(EntityTypeBuilder<OriginSkillEntity> builder)
    {
        builder.HasKey(e => new {e.OriginId, e.SkillId});

        builder.ToTable("origin_skills");

        builder.HasIndex(e => e.SkillId)
            .HasDatabaseName("IX_origin_skills_skillId");

        builder.Property(e => e.OriginId)
            .HasColumnName("originid");

        builder.Property(e => e.SkillId)
            .HasColumnName("skillid");

        builder.Property(e => e.Default)
            .HasColumnName("default");

        builder.HasOne(e => e.Origin)
            .WithMany(j => j.Skills)
            .HasForeignKey(e => e.OriginId)
            .HasConstraintName("FK_origin_skill_origin_originid")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Skill)
            .WithMany(s => s.OriginSkills)
            .HasForeignKey(e => e.SkillId)
            .HasConstraintName("FK_origin_skills_skillId_skills_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}