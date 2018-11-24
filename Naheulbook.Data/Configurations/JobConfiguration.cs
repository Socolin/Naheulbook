using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("job");

            builder.HasIndex(e => e.ParentJobId)
                .HasName("IX_job_parentjob");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.PlayerDescription)
                .HasColumnName("playerDescription");

            builder.Property(e => e.PlayerSummary)
                .HasColumnName("playerSummary");

            builder.Property(e => e.BaseAt)
                .HasColumnName("baseat");

            builder.Property(e => e.BaseEa)
                .HasColumnName("baseea");

            builder.Property(e => e.BaseEv)
                .HasColumnName("baseev");

            builder.Property(e => e.BasePrd)
                .HasColumnName("baseprd");

            builder.Property(e => e.BonusEv)
                .HasColumnName("bonusev");

            builder.Property(e => e.DiceEaLevelUp)
                .HasColumnName("diceealevelup");

            builder.Property(e => e.FactorEv)
                .HasColumnName("factorev");

            builder.Property(e => e.Information)
                .HasColumnName("informations");

            builder.Property(e => e.InternalName)
                .HasColumnName("internalname")
                .HasMaxLength(255);

            builder.Property(e => e.IsMagic)
                .HasColumnName("ismagic")
                .HasDefaultValueSql("false");

            builder.Property(e => e.MaxArmorPr)
                .HasColumnName("maxarmorpr");

            builder.Property(e => e.MaxLoad)
                .HasColumnName("maxload");

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(255);

            builder.Property(e => e.ParentJobId)
                .HasColumnName("parentjob");

            builder.Property(e => e.Flags)
                .HasColumnName("flags")
                .HasColumnType("json");
        }
    }

    public class JobBonusConfiguration : IEntityTypeConfiguration<JobBonus>
    {
        public void Configure(EntityTypeBuilder<JobBonus> builder)
        {
            builder.ToTable("job_bonus");

            builder.HasIndex(e => e.JobId)
                .HasName("IX_job_bonus_jobid");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Description)
                .IsRequired()
                .HasColumnName("description");

            builder.Property(e => e.JobId)
                .HasColumnName("jobid");

            builder.Property(e => e.Flags)
                .HasColumnName("flags")
                .HasColumnType("json");
        }
    }

    public class JobOriginBlacklistConfiguration : IEntityTypeConfiguration<JobOriginBlacklist>
    {
        public void Configure(EntityTypeBuilder<JobOriginBlacklist> builder)
        {
            builder.ToTable("job_origin_blacklist");

            builder.HasIndex(e => e.JobId)
                .HasName("IX_job_origin_blacklist_job");

            builder.HasIndex(e => e.OriginId)
                .HasName("IX_job_origin_blacklist_origin");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.JobId)
                .HasColumnName("job");

            builder.Property(e => e.OriginId)
                .HasColumnName("origin");
        }
    }

    public class JobOriginWhitelistConfiguration : IEntityTypeConfiguration<JobOriginWhitelist>
    {
        public void Configure(EntityTypeBuilder<JobOriginWhitelist> builder)
        {
            builder.ToTable("job_origin_whitelist");

            builder.HasIndex(e => e.JobId)
                .HasName("IX_job_origin_blacklist_job");

            builder.HasIndex(e => e.OriginId)
                .HasName("IX_job_origin_blacklist_origin");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.JobId)
                .HasColumnName("job");

            builder.Property(e => e.OriginId)
                .HasColumnName("origin");
        }
    }

    public class JobRequirementConfiguration : IEntityTypeConfiguration<JobRequirement>
    {
        public void Configure(EntityTypeBuilder<JobRequirement> builder)
        {
            builder.ToTable("job_requirement");

            builder.HasIndex(e => e.JobId)
                .HasName("IX_job_requirement_jobid");

            builder.HasIndex(e => e.StatName)
                .HasName("IX_job_requirement_stat");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.JobId)
                .HasColumnName("jobid");

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
                .HasConstraintName("FK_job_requirement_origin_originid")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Stat)
                .WithMany(e => e.JobRequirements)
                .HasForeignKey(e => e.StatName)
                .HasConstraintName("FK_job_requirement_stat_stat")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class JobRestrictConfiguration : IEntityTypeConfiguration<JobRestrict>
    {
        public void Configure(EntityTypeBuilder<JobRestrict> builder)
        {
            builder.ToTable("job_restrict");

            builder.HasIndex(e => e.JobId)
                .HasName("IX_job_restrict_jobid");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.JobId)
                .HasColumnName("jobid");

            builder.Property(e => e.Text)
                .IsRequired()
                .HasColumnName("text");

            builder.Property(e => e.Flags)
                .HasColumnName("flags")
                .HasColumnType("json");

            builder.HasOne(e => e.Job)
                .WithMany(e => e.Restrictions)
                .HasForeignKey(e => e.JobId)
                .HasConstraintName("FK_job_restrict_job_jobid")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
    {
        public void Configure(EntityTypeBuilder<JobSkill> builder)
        {
            builder.HasKey(e => new {e.JobId, e.SkillId});

            builder.ToTable("job_skill");

            builder.HasIndex(e => e.SkillId)
                .HasName("IX_job_skill_skillid");

            builder.Property(e => e.JobId)
                .HasColumnName("jobid");

            builder.Property(e => e.SkillId)
                .HasColumnName("skillid");

            builder.Property(e => e.Default)
                .HasColumnName("default");

            builder.HasOne(e => e.Job)
                .WithMany(j => j.Skills)
                .HasForeignKey(e => e.JobId)
                .HasConstraintName("FK_job_skill_job_jobid")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Skill)
                .WithMany(s => s.JobSkills)
                .HasForeignKey(e => e.SkillId)
                .HasConstraintName("FK_job_skill_skill_skillid")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}