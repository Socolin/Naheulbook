using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations;

public class SpecialityConfiguration : IEntityTypeConfiguration<SpecialityEntity>
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

public class SpecialityModifierConfiguration : IEntityTypeConfiguration<SpecialityModifierEntity>
{
    public void Configure(EntityTypeBuilder<SpecialityModifierEntity> builder)
    {
        builder.ToTable("speciality_modifiers");

        builder.HasIndex(e => e.SpecialityId)
            .HasDatabaseName("IX_speciality_modifiers_specialityId");

        builder.HasIndex(e => e.Stat)
            .HasDatabaseName("IX_speciality_modifier_stat");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.SpecialityId)
            .HasColumnName("specialityId");

        builder.Property(e => e.Stat)
            .IsRequired()
            .HasColumnName("stat")
            .HasMaxLength(64);

        builder.Property(e => e.Value)
            .HasColumnName("value");

        builder.HasOne(e => e.Speciality)
            .WithMany(s => s.Modifiers)
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_speciality_modifiers_specialityId_specialities_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SpecialitySpecialConfiguration : IEntityTypeConfiguration<SpecialitySpecialEntity>
{
    public void Configure(EntityTypeBuilder<SpecialitySpecialEntity> builder)
    {
        builder.ToTable("speciality_specials");

        builder.HasIndex(e => e.SpecialityId)
            .HasDatabaseName("IX_speciality_specials_specialityId");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.IsBonus)
            .HasColumnName("isbonus");

        builder.Property(e => e.SpecialityId)
            .HasColumnName("specialityId");

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.HasOne(e => e.Speciality)
            .WithMany(s => s.Specials)
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_speciality_specials_specialityId_specialities_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}