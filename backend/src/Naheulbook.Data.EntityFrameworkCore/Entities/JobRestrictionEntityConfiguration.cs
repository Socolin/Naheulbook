using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class JobRestrictionEntityConfiguration : IEntityTypeConfiguration<JobRestrictionEntity>
{
    public void Configure(EntityTypeBuilder<JobRestrictionEntity> builder)
    {
        builder.ToTable("job_restrictions");

        builder.HasIndex(e => e.JobId)
            .HasDatabaseName("IX_job_restrictions_jobId");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.JobId)
            .HasColumnName("jobId");

        builder.Property(e => e.Text)
            .IsRequired()
            .HasColumnName("text");

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.HasOne(e => e.Job)
            .WithMany(e => e.Restrictions)
            .HasForeignKey(e => e.JobId)
            .HasConstraintName("FK_job_restrictions_jobId_jobs_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}