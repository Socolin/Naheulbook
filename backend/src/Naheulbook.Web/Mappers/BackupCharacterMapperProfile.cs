using System.Linq;
using AutoMapper;
using Naheulbook.Data.Models;

namespace Naheulbook.Web.Mappers;

public class BackupCharacterMapperProfile : Profile
{
    public BackupCharacterMapperProfile()
    {
        CreateMap<CharacterEntity, Naheulbook.Core.Models.Backup.V1.BackupCharacter>()
            .ForMember(x => x.Stats, opt => opt.MapFrom(c => new Naheulbook.Core.Models.Backup.V1.BackupCharacter.BaseStats {Ad = c.Ad, Cou = c.Cou, Cha = c.Cha, Fo = c.Fo, Int = c.Int}))
            .ForMember(x => x.SkillIds, opt => opt.MapFrom(c => c.Skills.Select(x => x.SkillId)))
            .ForMember(x => x.JobIds, opt => opt.MapFrom(c => c.Jobs.Select(x => x.JobId)))
            .ForMember(x => x.SpecialitiesIds, opt => opt.MapFrom(c => c.Specialities.Select(x => x.SpecialityId)));
        CreateMap<CharacterModifierEntity, Naheulbook.Core.Models.Backup.V1.BackupCharacter.CharacterModifier>();
        CreateMap<CharacterModifierValueEntity, Naheulbook.Core.Models.Backup.V1.BackupCharacter.CharacterModifierValue>();

        CreateMap<ItemEntity, Naheulbook.Core.Models.Backup.V1.BackupCharacterItem>();
        CreateMap<ItemTemplateEntity, Naheulbook.Core.Models.Backup.V1.BackupItemTemplate>()
            .ForMember(x => x.Slots, opt => opt.MapFrom(c => c.Slots.Select(x => x.Slot.TechName)))
            .ForMember(x => x.Skills, opt => opt.MapFrom(c => c.Skills.Select(x => x.SkillId)))
            .ForMember(x => x.UnSkills, opt => opt.MapFrom(c => c.UnSkills.Select(x => x.SkillId)));
        CreateMap<ItemTemplateModifierEntity, Naheulbook.Core.Models.Backup.V1.BackupItemTemplate.BackupItemTemplateModifier>();
        CreateMap<ItemTemplateRequirementEntity, Naheulbook.Core.Models.Backup.V1.BackupItemTemplate.BackupItemTemplateRequirement>();
        CreateMap<ItemTemplateSkillModifierEntity, Naheulbook.Core.Models.Backup.V1.BackupItemTemplate.BackupItemTemplateSkillModifier>();
    }
}