using System.Linq;
using AutoMapper;
using Naheulbook.Data.Models;

namespace Naheulbook.Web.Mappers
{
    public class BackupCharacterMapperProfile : Profile
    {
        public BackupCharacterMapperProfile()
        {
            CreateMap<Character, Naheulbook.Core.Models.Backup.V1.BackupCharacter>()
                .ForMember(x => x.Stats, opt => opt.MapFrom(c => new Naheulbook.Core.Models.Backup.V1.BackupCharacter.BaseStats {Ad = c.Ad, Cou = c.Cou, Cha = c.Cha, Fo = c.Fo, Int = c.Int}))
                .ForMember(x => x.SkillIds, opt => opt.MapFrom(c => c.Skills.Select(x => x.SkillId)))
                .ForMember(x => x.JobIds, opt => opt.MapFrom(c => c.Jobs.Select(x => x.JobId)))
                .ForMember(x => x.SpecialitiesIds, opt => opt.MapFrom(c => c.Specialities.Select(x => x.SpecialityId)));
            CreateMap<CharacterModifier, Naheulbook.Core.Models.Backup.V1.BackupCharacter.CharacterModifier>();
            CreateMap<CharacterModifierValue, Naheulbook.Core.Models.Backup.V1.BackupCharacter.CharacterModifierValue>();

            CreateMap<Item, Naheulbook.Core.Models.Backup.V1.BackupCharacterItem>();
            CreateMap<ItemTemplate, Naheulbook.Core.Models.Backup.V1.BackupItemTemplate>()
                .ForMember(x => x.Slots, opt => opt.MapFrom(c => c.Slots.Select(x => x.Slot.TechName)))
                .ForMember(x => x.Skills, opt => opt.MapFrom(c => c.Skills.Select(x => x.SkillId)))
                .ForMember(x => x.UnSkills, opt => opt.MapFrom(c => c.UnSkills.Select(x => x.SkillId)));
            CreateMap<ItemTemplateModifier, Naheulbook.Core.Models.Backup.V1.BackupItemTemplate.BackupItemTemplateModifier>();
            CreateMap<ItemTemplateRequirement, Naheulbook.Core.Models.Backup.V1.BackupItemTemplate.BackupItemTemplateRequirement>();
            CreateMap<ItemTemplateSkillModifier, Naheulbook.Core.Models.Backup.V1.BackupItemTemplate.BackupItemTemplateSkillModifier>();
        }
    }
}