using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class OriginConfiguration : IEntityTypeConfiguration<Origin>
    {
        public void Configure(EntityTypeBuilder<Origin> builder)
        {
            builder.ToTable("origin");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Advantage)
                .HasColumnName("advantage");

            builder.Property(e => e.Description)
                .IsRequired()
                .HasColumnName("description");

            builder.Property(e => e.PlayerDescription)
                .HasColumnName("playerDescription");

            builder.Property(e => e.PlayerSummary)
                .HasColumnName("playerSummary");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);

            builder.Property(e => e.Size)
                .HasColumnName("size");

            builder.Property(e => e.Flags)
                .HasColumnName("flags")
                .HasColumnType("json");

            builder.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("json");
        }
    }

    public class OriginBonusConfiguration : IEntityTypeConfiguration<OriginBonus>
    {
        public void Configure(EntityTypeBuilder<OriginBonus> builder)
        {
            builder.ToTable("origin_bonus");

            builder.HasIndex(e => e.OriginId);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Description)
                .IsRequired()
                .HasColumnName("description");

            builder.Property(e => e.OriginId)
                .HasColumnName("originid");

            builder.Property(e => e.Flags)
                .HasColumnName("flags")
                .HasColumnType("json");
        }
    }

    public class OriginInfoConfiguration : IEntityTypeConfiguration<OriginInfo>
    {
        public void Configure(EntityTypeBuilder<OriginInfo> builder)
        {
            builder.ToTable("origin_info");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Description)
                .IsRequired()
                .HasColumnName("description");

            builder.Property(e => e.OriginId)
                .HasColumnName("originid");

            builder.Property(e => e.Title)
                .IsRequired()
                .HasColumnName("title")
                .HasMaxLength(255);

            builder.HasIndex(e => e.OriginId)
                .HasName("IX_origin_info_originid");

            builder.HasOne(e => e.Origin)
                .WithMany(o => o.Information)
                .HasForeignKey(e => e.OriginId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class OriginRequirementConfiguration : IEntityTypeConfiguration<OriginRequirement>
    {
        public void Configure(EntityTypeBuilder<OriginRequirement> builder)
        {
            builder.ToTable("origin_requirement");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.MaxValue)
                .HasColumnName("maxvalue");

            builder.Property(e => e.MinValue)
                .HasColumnName("minvalue");

            builder.Property(e => e.OriginId)
                .IsRequired()
                .HasColumnName("originid");

            builder.Property(e => e.StatName)
                .IsRequired()
                .HasColumnName("stat")
                .HasMaxLength(64);

            builder.HasIndex(e => e.OriginId)
                .HasName("IX_origin_requirement_originid");

            builder.HasIndex(e => e.StatName)
                .HasName("IX_origin_requirement_stat");

            builder.HasOne(e => e.Origin)
                .WithMany(o => o.Requirements)
                .HasForeignKey(e => e.OriginId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class OriginRestrictConfiguration : IEntityTypeConfiguration<OriginRestrict>
    {
        public void Configure(EntityTypeBuilder<OriginRestrict> builder)
        {
            builder.ToTable("origin_restrict");

            builder.HasIndex(e => e.OriginId)
                .HasName("IX_origin_restrict_originid");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.OriginId)
                .HasColumnName("originid");

            builder.Property(e => e.Text)
                .IsRequired()
                .HasColumnName("text");

            builder.Property(e => e.Flags)
                .HasColumnName("flags")
                .HasColumnType("json");
        }
    }

    public class OriginSkillConfiguration : IEntityTypeConfiguration<OriginSkill>
    {
        public void Configure(EntityTypeBuilder<OriginSkill> builder)
        {
            builder.HasKey(e => new {e.OriginId, e.SkillId});

            builder.ToTable("origin_skill");

            builder.HasIndex(e => e.SkillId)
                .HasName("IX_origin_skill_skillid");

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
                .HasConstraintName("FK_origin_skill_skill_skillid")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class OriginRandomNameUrlConfiguration : IEntityTypeConfiguration<OriginRandomNameUrl>
    {
        public void Configure(EntityTypeBuilder<OriginRandomNameUrl> builder)
        {
            builder.ToTable("origin_random_name_urls");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Sex)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("sex");

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("url");

            builder.Property(x => x.OriginId)
                .HasColumnName("originId");

            builder.HasOne(x => x.Origin)
                .WithMany()
                .HasForeignKey(x => x.OriginId);
        }
    }
}