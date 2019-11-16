using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class CharacterConfiguration : IEntityTypeConfiguration<Character>
    {
        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.ToTable("characters");

            builder.HasKey(x => x.Id);

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
                .HasColumnName("user");
            builder.Property(e => e.OriginId)
                .IsRequired()
                .HasColumnName("origin");
            builder.Property(e => e.GroupId)
                .IsRequired(false)
                .HasColumnName("group");
            builder.Property(e => e.TargetedCharacterId)
                .IsRequired(false)
                .HasColumnName("targetcharacterid");
            builder.Property(e => e.TargetedMonsterId)
                .IsRequired(false)
                .HasColumnName("targetmonsterid");

            builder.HasOne(e => e.Owner)
                .WithMany(e => e.Characters)
                .HasForeignKey(e => e.OwnerId)
                .HasConstraintName("FK_character_user_user");

            builder.HasOne(e => e.Origin)
                .WithMany()
                .HasForeignKey(e => e.OriginId)
                .HasConstraintName("FK_character_origin_origin");

            builder.HasOne(e => e.Group!)
                .WithMany(e => e.Characters)
                .HasForeignKey(e => e.GroupId)
                .HasConstraintName("FK_character_group_group");

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

    public class CharacterJobConfiguration : IEntityTypeConfiguration<CharacterJob>
    {
        public void Configure(EntityTypeBuilder<CharacterJob> builder)
        {
            builder.ToTable("character_jobs");

            builder.HasKey(e => new {e.CharacterId, e.JobId});

            builder.Property(e => e.CharacterId)
                .HasColumnName("character");

            builder.Property(e => e.JobId)
                .HasColumnName("job");

            builder.Property(e => e.Order)
                .HasColumnName("order");

            builder.HasOne(e => e.Character)
                .WithMany(e => e.Jobs)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("character_job_character_id_fk");

            builder.HasOne(e => e.Job)
                .WithMany()
                .HasForeignKey(e => e.JobId)
                .HasConstraintName("character_job_job_id_fk");
        }
    }

    public class CharacterModifierConfiguration : IEntityTypeConfiguration<CharacterModifier>
    {
        public void Configure(EntityTypeBuilder<CharacterModifier> builder)
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
                .HasColumnName("character");

            builder.HasOne(e => e.Character!)
                .WithMany(e => e.Modifiers)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_character_modifier_character_character");

            builder.HasMany(e => e.Values)
                .WithOne()
                .HasForeignKey(e => e.CharacterModifierId)
                .HasConstraintName("FK_character_modifier_value_character_modifier_charactermodifier");
        }
    }

    public class CharacterModifierValueConfiguration : IEntityTypeConfiguration<CharacterModifierValue>
    {
        public void Configure(EntityTypeBuilder<CharacterModifierValue> builder)
        {
            builder.ToTable("character_modifier_values");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.StatName)
                .HasColumnName("stat");
            builder.Property(e => e.Type)
                .HasColumnName("type");
            builder.Property(e => e.Value)
                .HasColumnName("value");
            builder.Property(e => e.CharacterModifierId)
                .HasColumnName("charactermodifier");
        }
    }

    public class CharacterSkillConfiguration : IEntityTypeConfiguration<CharacterSkill>
    {
        public void Configure(EntityTypeBuilder<CharacterSkill> builder)
        {
            builder.ToTable("character_skills");

            builder.HasKey(e => new {e.SkillId, e.CharacterId});

            builder.HasIndex(e => e.CharacterId)
                .HasName("IX_character_skills_character");

            builder.HasIndex(e => e.SkillId)
                .HasName("IX_character_skills_skillId");

            builder.Property(e => e.CharacterId)
                .HasColumnName("character");

            builder.Property(e => e.SkillId)
                .HasColumnName("skillId");

            builder.HasOne(e => e.Character)
                .WithMany(e => e.Skills)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_character_skills_character_character");

            builder.HasOne(e => e.Skill)
                .WithMany()
                .HasForeignKey(e => e.SkillId)
                .HasConstraintName("FK_character_skills_skillId_skills_id");
        }
    }

    public class CharacterSpecialityConfiguration : IEntityTypeConfiguration<CharacterSpeciality>
    {
        public void Configure(EntityTypeBuilder<CharacterSpeciality> builder)
        {
            builder.ToTable("character_specialities");

            builder.HasKey(e => new {e.SpecialityId, e.CharacterId});

            builder.Property(e => e.CharacterId)
                .HasColumnName("character");
            builder.Property(e => e.SpecialityId)
                .HasColumnName("speciality");

            builder.HasOne(e => e.Character)
                .WithMany(e => e.Specialities)
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_character_speciality_character_character");

            builder.HasOne(e => e.Speciality)
                .WithMany()
                .HasForeignKey(e => e.SpecialityId)
                .HasConstraintName("FK_character_speciality_speciality_speciality");
        }
    }

    public class CharacterHistoryEntryConfiguration : IEntityTypeConfiguration<CharacterHistoryEntry>
    {
        public void Configure(EntityTypeBuilder<CharacterHistoryEntry> builder)
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