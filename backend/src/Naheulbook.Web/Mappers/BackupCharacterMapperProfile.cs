using System.Linq;
using AutoMapper;
using Naheulbook.Core.Features.Character.Backup.V1;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Web.Mappers;

public class BackupCharacterMapperProfile : Profile
{
    public BackupCharacterMapperProfile()
    {
        CreateMap<CharacterEntity, BackupCharacter>()
            .ForMember(x => x.Stats, opt => opt.MapFrom(c => new BackupCharacter.BaseStats {Ad = c.Ad, Cou = c.Cou, Cha = c.Cha, Fo = c.Fo, Int = c.Int}))
            .ForMember(x => x.SkillIds, opt => opt.MapFrom(c => c.Skills.Select(x => x.SkillId)))
            .ForMember(x => x.JobIds, opt => opt.MapFrom(c => c.Jobs.Select(x => x.JobId)))
            .ForMember(x => x.SpecialitiesIds, opt => opt.MapFrom(c => c.Specialities.Select(x => x.SpecialityId)));
        CreateMap<CharacterModifierEntity, BackupCharacter.CharacterModifier>();
        CreateMap<CharacterModifierValueEntity, BackupCharacter.CharacterModifierValue>();

        CreateMap<ItemEntity, BackupCharacterItem>();
        CreateMap<ItemTemplateEntity, BackupItemTemplate>()
            .ForMember(x => x.Slots, opt => opt.MapFrom(c => c.Slots.Select(x => x.Slot.TechName)))
            .ForMember(x => x.Skills, opt => opt.MapFrom(c => c.Skills.Select(x => x.SkillId)))
            .ForMember(x => x.UnSkills, opt => opt.MapFrom(c => c.UnSkills.Select(x => x.SkillId)));
        CreateMap<ItemTemplateModifierEntity, BackupItemTemplate.BackupItemTemplateModifier>();
        CreateMap<ItemTemplateRequirementEntity, BackupItemTemplate.BackupItemTemplateRequirement>();
        CreateMap<ItemTemplateSkillModifierEntity, BackupItemTemplate.BackupItemTemplateSkillModifier>();
    }
}