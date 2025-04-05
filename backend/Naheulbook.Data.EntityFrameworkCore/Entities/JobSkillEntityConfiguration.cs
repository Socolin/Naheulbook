using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class JobSkillEntityConfiguration : IEntityTypeConfiguration<JobSkillEntity>
{
    public void Configure(EntityTypeBuilder<JobSkillEntity> builder)
    {
        builder.HasKey(e => new {e.JobId, e.SkillId});

        builder.ToTable("job_skills");

        builder.HasIndex(e => e.SkillId)
            .HasDatabaseName("IX_job_skills_skillId");

        builder.HasIndex(e => e.JobId)
            .HasDatabaseName("IX_job_skills_jobId");

        builder.Property(e => e.JobId)
            .HasColumnName("jobId");

        builder.Property(e => e.SkillId)
            .HasColumnName("skillId");

        builder.Property(e => e.Default)
            .HasColumnName("default");

        builder.HasOne(e => e.Job)
            .WithMany(j => j.Skills)
            .HasForeignKey(e => e.JobId)
            .HasConstraintName("FK_job_skills_jobId_jobs_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Skill)
            .WithMany(s => s.JobSkills)
            .HasForeignKey(e => e.SkillId)
            .HasConstraintName("FK_job_skills_skillId_skills_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}