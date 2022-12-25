using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class CharacterConfiguration : IEntityTypeConfiguration<CharacterEntity>
    {
        public void Configure(EntityTypeBuilder<CharacterEntity> builder)
        {
            builder.ToTable("characters");

            builder.HasKey(x => x.Id);

            builder.HasIndex(e => e.GroupId)
                .HasDatabaseName("IX_characters_groupId");
            builder.HasIndex(e => e.OriginId)
                .HasDatabaseName("IX_characters_originId");
            builder.HasIndex(e => e.OwnerId)
                .HasDatabaseName("IX_characters_userId");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);
            builder.Property(e => e.Sex)
                .IsRequired()
                .HasColumnName("sexe");
            builder.Property(e => e.IsActive)
                .HasColumnName("active");
            builder.Property(e => e.IsNpc)
                .HasColumnName("isnpc");
            builder.Property(e => e.Color)
                .HasDefaultValue("22DD22")
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .HasColumnName("color");

            builder.Property(e => e.Ad)
                .HasColumnName("ad");
            builder.Property(e => e.Cha)
                .HasColumnName("cha");
            builder.Property(e => e.Cou)
                .HasColumnName("cou");
            builder.Property(e => e.Fo)
                .HasColumnName("fo");
            builder.Property(e => e.Int)
                .HasColumnName("int");

            builder.Property(e => e.Ev)
                .IsRequired(false)
                .HasColumnName("ev");
            builder.Property(e => e.Ea)
                .IsRequired(false)
                .HasColumnName("ea");

            builder.Property(e => e.Notes)
                .IsRequired(false)
                .HasColumnName("notes");

            builder.Property(e => e.GmData)
                .IsRequired(false)
                .HasColumnName("gmdata");
            builder.Property(e => e.FatePoint)
                .HasColumnName("fatepoint");
            builder.Property(e => e.StatBonusAd)
                .HasColumnName("statbonusad");

            builder.Property(e => e.Level)
                .HasColumnName("level");
            builder.Property(e => e.Experience)
                .HasColumnName("experience");

            builder.Property(e => e.OwnerId)
                .HasColumnName("userId");
            builder.Property(e => e.OriginId)
                .IsRequired()
                .HasColumnName("originId");
            builder.Property(e => e.GroupId)
                .IsRequired(false)
                .HasColumnName("groupId");
            builder.Property(e => e.TargetedCharacterId)
                .IsRequired(false)
                .HasColumnName("targetcharacterid");
            builder.Property(e => e.TargetedMonsterId)
                .IsRequired(false)
                .HasColumnName("targetmonsterid");

            builder.HasOne(e => e.Owner)
                .WithMany(e => e.Characters)
                .HasForeignKey(e => e.OwnerId)
                .HasConstraintName("FK_characters_userId_users_id");

            builder.HasOne(e => e.Origin)
                .WithMany()
                .HasForeignKey(e => e.OriginId)
                .HasConstraintName("FK_characters_originId_origins_id");

            builder.HasOne(e => e.Group!)
                .WithMany(e => e.Characters)
                .HasForeignKey(e => e.GroupId)
                .HasConstraintName("FK_characters_groupId_groups_id");

            builder.HasOne(e => e.TargetedCharacter)
                .WithMany()
                .HasForeignKey(e => e.TargetedCharacterId)
                .HasConstraintName("IX_character_targetcharacterid");

            builder.HasOne(e => e.TargetedMonster)
                .WithMany()
                .HasForeignKey(e => e.TargetedMonsterId)
                .HasConstraintName("FK_character_monster_targetmonsterid");
        }
    }

    public class CharacterJobConfiguration : IEntityTypeConfiguration<CharacterJobEntity>
    {
        public void Configure(EntityTypeBuilder<CharacterJobEntity> builder)
        {
            builder.ToTable("character_jobs");

            builder.HasKey(e => new {e.CharacterId, e.JobId});

            builder.HasIndex(e => e.JobId)
                .HasDatabaseName("IX_character_jobs_jobId");

            builder.HasIndex(e => e.CharacterId)
                .HasDatabaseName("IX_character_jobs_characterId");

            builder.Property(e => e.CharacterId)
                .HasColumnName("characterId");

            builder.Property(e => e.JobId)
                .HasColumnName("jobId");

            builder.Property(e => e.Order)
                .HasColumnName("order");

            builder.HasOne(e => e.Character)
                .WithMany(e => e.Jobs)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_character_jobs_characterId_characters_id");

            builder.HasOne(e => e.Job)
                .WithMany()
                .HasForeignKey(e => e.JobId)
                .HasConstraintName("FK_character_jobs_jobId_jobs_id");
        }
    }

    public class CharacterModifierConfiguration : IEntityTypeConfiguration<CharacterModifierEntity>
    {
        public void Configure(EntityTypeBuilder<CharacterModifierEntity> builder)
        {
            builder.ToTable("character_modifiers");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Name)
                .HasColumnName("name");
            builder.Property(e => e.Type)
                .HasColumnName("type");
            builder.Property(e => e.Description)
                .HasColumnName("description");

            builder.Property(e => e.IsActive)
                .HasColumnName("active");
            builder.Property(e => e.Permanent)
                .HasColumnName("permanent");
            builder.Property(e => e.Reusable)
                .HasColumnName("reusable");

            builder.Property(e => e.DurationType)
                .HasColumnName("durationtype");

            builder.Property(e => e.TimeDuration)
                .IsRequired(false)
                .HasColumnName("timeduration");
            builder.Property(e => e.LapCount)
                .IsRequired(false)
                .HasColumnName("lapcount");
            builder.Property(e => e.CombatCount)
                .IsRequired(false)
                .HasColumnName("combatcount");
            builder.Property(e => e.LapCountDecrement)
                .IsRequired(false)
                .HasColumnName("lapCountDecrement");

            builder.Property(e => e.CurrentLapCount)
                .IsRequired(false)
                .HasColumnName("currentlapcount");
            builder.Property(e => e.CurrentCombatCount)
                .IsRequired(false)
                .HasColumnName("currentcombatcount");
            builder.Property(e => e.CurrentTimeDuration)
                .IsRequired(false)
                .HasColumnName("currenttimeduration");
            builder.Property(e => e.Duration)
                .IsRequired(false)
                .HasColumnName("duration");

            builder.Property(e => e.CharacterId)
                .HasColumnName("characterId");

            builder.HasOne(e => e.Character!)
                .WithMany(e => e.Modifiers)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_character_modifiers_characterId_characters_id");

            builder.HasMany(e => e.Values)
                .WithOne()
                .HasForeignKey(e => e.CharacterModifierId)
                .HasConstraintName("FK_character_modifier_values_characterModifierId");
        }
    }

    public class CharacterModifierValueConfiguration : IEntityTypeConfiguration<CharacterModifierValueEntity>
    {
        public void Configure(EntityTypeBuilder<CharacterModifierValueEntity> builder)
        {
            builder.ToTable("character_modifier_values");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.StatName)
                .HasDatabaseName("IX_character_modifier_value_stat");
            builder.HasIndex(e => e.CharacterModifierId)
                .HasDatabaseName("IX_character_modifier_values_characterModifierId");

            builder.Property(e => e.StatName)
                .HasColumnName("stat");
            builder.Property(e => e.Type)
                .HasColumnName("type");
            builder.Property(e => e.Value)
                .HasColumnName("value");
            builder.Property(e => e.CharacterModifierId)
                .HasColumnName("characterModifierId");
        }
    }

    public class CharacterSkillConfiguration : IEntityTypeConfiguration<CharacterSkillEntity>
    {
        public void Configure(EntityTypeBuilder<CharacterSkillEntity> builder)
        {
            builder.ToTable("character_skills");

            builder.HasKey(e => new {e.SkillId, e.CharacterId});

            builder.HasIndex(e => e.CharacterId)
                .HasDatabaseName("IX_character_skills_characterId");

            builder.HasIndex(e => e.SkillId)
                .HasDatabaseName("IX_character_skills_skillId");

            builder.Property(e => e.CharacterId)
                .HasColumnName("characterId");

            builder.Property(e => e.SkillId)
                .HasColumnName("skillId");

            builder.HasOne(e => e.Character)
                .WithMany(e => e.Skills)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_character_skills_characterId_characters_id");

            builder.HasOne(e => e.Skill)
                .WithMany()
                .HasForeignKey(e => e.SkillId)
                .HasConstraintName("FK_character_skills_skillId_skills_id");
        }
    }

    public class CharacterSpecialityConfiguration : IEntityTypeConfiguration<CharacterSpecialityEntity>
    {
        public void Configure(EntityTypeBuilder<CharacterSpecialityEntity> builder)
        {
            builder.ToTable("character_specialities");

            builder.HasKey(e => new {e.SpecialityId, e.CharacterId});

            builder.HasIndex(e => e.SpecialityId)
                .HasDatabaseName("IX_character_specialities_specialityId");
            builder.HasIndex(e => e.CharacterId)
                .HasDatabaseName("IX_character_specialities_characterId");

            builder.Property(e => e.CharacterId)
                .HasColumnName("characterId");
            builder.Property(e => e.SpecialityId)
                .HasColumnName("specialityId");

            builder.HasOne(e => e.Character)
                .WithMany(e => e.Specialities)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_character_specialities_characterId_characters_id");

            builder.HasOne(e => e.Speciality)
                .WithMany()
                .HasForeignKey(e => e.SpecialityId)
                .HasConstraintName("FK_character_specialities_specialityId_specialities_id");
        }
    }

    public class CharacterHistoryEntryConfiguration : IEntityTypeConfiguration<CharacterHistoryEntryEntity>
    {
        public void Configure(EntityTypeBuilder<CharacterHistoryEntryEntity> builder)
        {
            builder.ToTable("character_history_entries");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");
            builder.Property(e => e.CharacterId)
                .HasColumnName("character");
            builder.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("json");
            builder.Property(e => e.Date)
                .HasColumnName("date");
            builder.Property(e => e.Info)
                .HasColumnName("info");
            builder.Property(e => e.Action)
                .HasColumnName("action");
            builder.Property(e => e.Gm)
                .HasColumnName("gm");
            builder.Property(e => e.EffectId)
                .HasColumnName("effect");
            builder.Property(e => e.ItemId)
                .HasColumnName("item");
            builder.Property(e => e.CharacterModifierId)
                .HasColumnName("modifier");

            builder.HasOne(e => e.Character)
                .WithMany(g => g.HistoryEntries)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_character_history_character_character");

            builder.HasOne(e => e.CharacterModifier)
                .WithMany()
                .HasForeignKey(e => e.CharacterModifierId)
                .HasConstraintName("FK_character_history_character_modifier_modifier");

            builder.HasOne(e => e.Effect)
                .WithMany()
                .HasForeignKey(e => e.EffectId)
                .HasConstraintName("FK_character_history_effect_effect");

            builder.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .HasConstraintName("FK_character_history_item_item");
        }
    }
}