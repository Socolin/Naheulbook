using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations;

public class OriginConfiguration : IEntityTypeConfiguration<OriginEntity>
{
    public void Configure(EntityTypeBuilder<OriginEntity> builder)
    {
        builder.ToTable("origins");

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
        builder.ToTable("origin_bonuses");

        builder.HasIndex(e => e.OriginId);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.HasIndex(e => e.OriginId)
            .HasDatabaseName("IX_origin_bonuses_originId");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.OriginId)
            .HasColumnName("originId");

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.HasOne(e => e.Origin)
            .WithMany(o => o.Bonuses)
            .HasForeignKey(o => o.OriginId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_origin_bonuses_originId_origins_id");
    }
}

public class OriginInfoConfiguration : IEntityTypeConfiguration<OriginInfoEntity>
{
    public void Configure(EntityTypeBuilder<OriginInfoEntity> builder)
    {
        builder.ToTable("origin_information");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.OriginId)
            .HasColumnName("originId");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasColumnName("title")
            .HasMaxLength(255);

        builder.HasIndex(e => e.OriginId)
            .HasDatabaseName("IX_origin_information_originId");

        builder.HasOne(e => e.Origin)
            .WithMany(o => o.Information)
            .HasForeignKey(e => e.OriginId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_origin_information_originId_origins_id");
    }
}

public class OriginRequirementConfiguration : IEntityTypeConfiguration<OriginRequirementEntity>
{
    public void Configure(EntityTypeBuilder<OriginRequirementEntity> builder)
    {
        builder.ToTable("origin_requirements");

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
            .HasDatabaseName("IX_origin_requirements_originId");

        builder.HasIndex(e => e.StatName)
            .HasDatabaseName("IX_origin_requirement_stat");

        builder.HasOne(e => e.Origin)
            .WithMany(o => o.Requirements)
            .HasForeignKey(e => e.OriginId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_origin_requirements_originId_origins_id");
    }
}

public class OriginRestrictConfiguration : IEntityTypeConfiguration<OriginRestrictEntity>
{
    public void Configure(EntityTypeBuilder<OriginRestrictEntity> builder)
    {
        builder.ToTable("origin_restrictions");

        builder.HasIndex(e => e.OriginId)
            .HasDatabaseName("IX_origin_restrictions_originId");

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

        builder.HasOne(e => e.Origin)
            .WithMany(o => o.Restrictions)
            .HasForeignKey(o => o.OriginId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_origin_restrictions_originId_origins_id");
    }
}

public class OriginSkillConfiguration : IEntityTypeConfiguration<OriginSkillEntity>
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

public class OriginRandomNameUrlConfiguration : IEntityTypeConfiguration<OriginRandomNameUrlEntity>
{
    public void Configure(EntityTypeBuilder<OriginRandomNameUrlEntity> builder)
    {
        builder.ToTable("origin_random_name_urls");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.OriginId)
            .HasDatabaseName("IX_origin_random_name_urls_originId");

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

        builder.HasOne(e => e.Origin)
            .WithMany()
            .HasForeignKey(e => e.OriginId)
            .HasConstraintName("FK_origin_random_name_urls_originId_origins_id")
            .OnDelete(DeleteBehavior.Cascade);

    }
}