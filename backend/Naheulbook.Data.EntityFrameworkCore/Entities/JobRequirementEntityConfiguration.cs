using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class JobRequirementEntityConfiguration : IEntityTypeConfiguration<JobRequirementEntity>
{
    public void Configure(EntityTypeBuilder<JobRequirementEntity> builder)
    {
        builder.ToTable("job_requirements");

        builder.HasIndex(e => e.JobId)
            .HasDatabaseName("IX_job_requirements_jobId");

        builder.HasIndex(e => e.StatName)
            .HasDatabaseName("IX_job_requirement_stat");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.JobId)
            .HasColumnName("jobId");

        builder.Property(e => e.MaxValue)
            .HasColumnName("maxvalue");

        builder.Property(e => e.MinValue)
            .HasColumnName("minvalue");

        builder.Property(e => e.StatName)
            .IsRequired()
            .HasColumnName("stat")
            .HasMaxLength(64);

        builder.HasOne(e => e.Job)
            .WithMany(e => e.Requirements)
            .HasForeignKey(e => e.JobId)
            .HasConstraintName("FK_job_requirements_jobId_jobs_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Stat)
            .WithMany(e => e.JobRequirements)
            .HasForeignKey(e => e.StatName)
            .HasConstraintName("FK_job_requirement_stat_stat")
            .OnDelete(DeleteBehavior.Restrict);
    }
}