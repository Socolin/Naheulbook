using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
    {
        public void Configure(EntityTypeBuilder<Speciality> builder)
        {
            builder.ToTable("specialities");

            builder.HasIndex(e => e.JobId)
                .HasName("IX_speciality_job");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Description)
                .IsRequired()
                .HasColumnName("description");

            builder.Property(e => e.JobId)
                .HasColumnName("job");

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
                .HasConstraintName("FK_speciality_job_job")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class SpecialityModifierConfiguration : IEntityTypeConfiguration<SpecialityModifier>
    {
        public void Configure(EntityTypeBuilder<SpecialityModifier> builder)
        {
            builder.ToTable("speciality_modifiers");

            builder.HasIndex(e => e.SpecialityId)
                .HasName("IX_speciality_modifier_speciality");

            builder.HasIndex(e => e.Stat)
                .HasName("IX_speciality_modifier_stat");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.SpecialityId)
                .HasColumnName("speciality");

            builder.Property(e => e.Stat)
                .IsRequired()
                .HasColumnName("stat")
                .HasMaxLength(64);

            builder.Property(e => e.Value)
                .HasColumnName("value");

            builder.HasOne(e => e.Speciality)
                .WithMany(s => s.Modifiers)
                .HasForeignKey(e => e.SpecialityId)
                .HasConstraintName("FK_speciality_modifier_speciality_speciality")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class SpecialitySpecialConfiguration : IEntityTypeConfiguration<SpecialitySpecial>
    {
        public void Configure(EntityTypeBuilder<SpecialitySpecial> builder)
        {
            builder.ToTable("speciality_specials");

            builder.HasIndex(e => e.SpecialityId)
                .HasName("IX_speciality_special_speciality");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Description)
                .IsRequired()
                .HasColumnName("description");

            builder.Property(e => e.IsBonus)
                .HasColumnName("isbonus");

            builder.Property(e => e.SpecialityId)
                .HasColumnName("speciality");

            builder.Property(e => e.Flags)
                .HasColumnName("flags")
                .HasColumnType("json");

            builder.HasOne(e => e.Speciality)
                .WithMany(s => s.Specials)
                .HasForeignKey(e => e.SpecialityId)
                .HasConstraintName("FK_speciality_special_speciality_speciality")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}