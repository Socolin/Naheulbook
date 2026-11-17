using Naheulbook.Core.Features.Character.Backup.V1;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterBackupMapper
{
    BackupCharacter ToBackup(CharacterEntity character);
    BackupCharacter.CharacterModifier ToBackup(CharacterModifierEntity characterModifier);
    BackupCharacterItem ToBackup(ItemEntity item);
    BackupItemTemplate ToBackup(ItemTemplateEntity itemTemplate);
}

public class CharacterBackupMapper : ICharacterBackupMapper
{
    public BackupCharacter ToBackup(CharacterEntity character)
    {
        return new BackupCharacter
        {
            Name = character.Name,
            Sex = character.Sex,
            Ev = character.Ev,
            Ea = character.Ea,
            FatePoint = character.FatePoint,
            StatBonusAd = character.StatBonusAd,
            Notes = character.Notes,
            Level = character.Level,
            Experience = character.Experience,
            OriginId = character.OriginId,
            Stats = new BackupCharacter.BaseStats
            {
                Ad = character.Ad,
                Cou = character.Cou,
                Cha = character.Cha,
                Fo = character.Fo,
                Int = character.Int,
            },
            JobIds = character.Jobs.Select(j => j.JobId).ToList(),
            SkillIds = character.Skills.Select(s => s.SkillId).ToList(),
            SpecialitiesIds = character.Specialities.Select(s => s.SpecialityId).ToList(),
            Modifiers = character.Modifiers.Select(ToBackup).ToList(),
            Items = character.Items.Select(ToBackup).ToList(),
        };
    }

    public BackupCharacter.CharacterModifier ToBackup(CharacterModifierEntity characterModifier)
    {
        return new BackupCharacter.CharacterModifier
        {
            Name = characterModifier.Name,
            IsActive = characterModifier.IsActive,
            Description = characterModifier.Description,
            Type = characterModifier.Type,
            Permanent = characterModifier.Permanent,
            Reusable = characterModifier.Reusable,
            DurationType = characterModifier.DurationType,
            Duration = characterModifier.Duration,
            TimeDuration = characterModifier.TimeDuration,
            CombatCount = characterModifier.CombatCount,
            LapCount = characterModifier.LapCount,
            LapCountDecrement = characterModifier.LapCountDecrement,
            CurrentCombatCount = characterModifier.CurrentCombatCount,
            CurrentTimeDuration = characterModifier.CurrentTimeDuration,
            CurrentLapCount = characterModifier.CurrentLapCount,
            Values = characterModifier.Values.Select(v => new BackupCharacter.CharacterModifierValue
            {
                StatName = v.StatName,
                Type = v.Type,
                Value = v.Value,
            }).ToList(),
        };
    }

    public BackupCharacterItem ToBackup(ItemEntity item)
    {
        return new BackupCharacterItem
        {
            Data = item.Data,
            Modifiers = item.Modifiers,
            ContainerId = item.ContainerId,
            ItemTemplate = ToBackup(item.ItemTemplate),
        };
    }

    public BackupItemTemplate ToBackup(ItemTemplateEntity itemTemplate)
    {
        return new BackupItemTemplate
        {
            Id = itemTemplate.Id,
            Name = itemTemplate.Name,
            TechName = itemTemplate.TechName,
            SourceUserNameCache = itemTemplate.SourceUserNameCache,
            Data = itemTemplate.Data,
            SubCategoryId = itemTemplate.SubCategoryId,
            Modifiers = itemTemplate.Modifiers.Select(m => new BackupItemTemplate.BackupItemTemplateModifier
            {
                Value = m.Value,
                Special = m.Special,
                Type = m.Type,
                RequiredJobId = m.RequiredJobId,
                RequiredOriginId = m.RequiredOriginId,
                StatName = m.StatName,
            }).ToList(),
            Requirements = itemTemplate.Requirements.Select(r => new BackupItemTemplate.BackupItemTemplateRequirement
            {
                MinValue = r.MinValue,
                MaxValue = r.MaxValue,
                StatName = r.StatName,
            }).ToList(),
            SkillModifiers = itemTemplate.SkillModifiers.Select(s => new BackupItemTemplate.BackupItemTemplateSkillModifier
            {
                SkillId = s.SkillId,
                Value = s.Value,
            }).ToList(),
            Slots = itemTemplate.Slots.Select(s => s.Slot.TechName).ToList(),
            Skills = itemTemplate.Skills.Select(s => s.SkillId).ToList(),
            UnSkills = itemTemplate.UnSkills.Select(s => s.SkillId).ToList(),
        };
    }
}
