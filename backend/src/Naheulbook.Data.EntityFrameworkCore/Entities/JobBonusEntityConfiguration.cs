using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class JobBonusEntityConfiguration : IEntityTypeConfiguration<JobBonusEntity>
{
    public void Configure(EntityTypeBuilder<JobBonusEntity> builder)
    {
        builder.ToTable("job_bonuses");

        builder.HasIndex(e => e.JobId)
            .HasDatabaseName("IX_job_bonuses_jobId");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.JobId)
            .HasColumnName("jobId");

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.HasOne(e => e.Job)
            .WithMany(e => e.Bonuses)
            .HasForeignKey(e => e.JobId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_job_bonuses_jobId_jobs_id");
    }
}