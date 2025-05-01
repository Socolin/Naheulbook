using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpecialityEntityConfiguration : IEntityTypeConfiguration<SpecialityEntity>
{
    public void Configure(EntityTypeBuilder<SpecialityEntity> builder)
    {
        builder.ToTable("specialities");

        builder.HasIndex(e => e.JobId)
            .HasDatabaseName("IX_specialities_jobId");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.JobId)
            .HasColumnName("jobId");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.HasOne(e => e.Job)
            .WithMany(j => j.Specialities)
            .HasForeignKey(e => e.JobId)
            .HasConstraintName("FK_specialities_jobId_jobs_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}