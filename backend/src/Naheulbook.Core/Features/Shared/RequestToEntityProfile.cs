using AutoMapper;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Newtonsoft.Json;

namespace Naheulbook.Core.Features.Shared;

public class RequestToEntityProfile : Profile
{
    public RequestToEntityProfile()
    {
        CreateMap<ItemTemplateRequest, ItemTemplateEntity>()
            .ForMember(m => m.Id, opt => opt.Ignore())
            .ForMember(m => m.CleanName, opt => opt.MapFrom(i => StringCleanupHelper.RemoveAccents(i.Name)))
            .ForMember(m => m.SubCategory, opt => opt.Ignore())
            .ForMember(m => m.SourceUserId, opt => opt.Ignore())
            .ForMember(m => m.SourceUser, opt => opt.Ignore())
            .ForMember(m => m.Skills, opt => opt.MapFrom(r => r.SkillIds.Select(skillId => new ItemTemplateSkillEntity {SkillId = skillId})))
            .ForMember(m => m.UnSkills, opt => opt.MapFrom(r => r.UnSkillIds.Select(skillId => new ItemTemplateUnSkillEntity {SkillId = skillId})))
            .ForMember(m => m.SourceUser, opt => opt.Ignore())
            .ForMember(m => m.Data, opt => opt.MapFrom(i => i.Data.ToString(Formatting.None)));
        CreateMap<ItemTemplateModifierRequest, ItemTemplateModifierEntity>()
            .ForMember(m => m.Id, opt => opt.Ignore())
            .ForMember(m => m.RequiredJobId, opt => opt.MapFrom(im => im.JobId))
            .ForMember(m => m.RequiredOriginId, opt => opt.MapFrom(im => im.OriginId))
            .ForMember(m => m.Special, opt => opt.MapFrom(im => im.Special == null ? null : string.Join(',', im.Special)))
            .ForMember(m => m.StatName, opt => opt.MapFrom(im => im.Stat))
            .ForMember(m => m.Stat, opt => opt.Ignore())
            .ForMember(m => m.RequiredJob, opt => opt.Ignore())
            .ForMember(m => m.RequiredOrigin, opt => opt.Ignore())
            .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
        CreateMap<ItemTemplateRequirementRequest, ItemTemplateRequirementEntity>()
            .ForMember(m => m.Id, opt => opt.Ignore())
            .ForMember(m => m.StatName, opt => opt.MapFrom(ir => ir.Stat))
            .ForMember(m => m.Stat, opt => opt.Ignore())
            .ForMember(m => m.MinValue, opt => opt.MapFrom(ir => ir.Min))
            .ForMember(m => m.MaxValue, opt => opt.MapFrom(ir => ir.Max))
            .ForMember(m => m.ItemTemplateId, opt => opt.Ignore())
            .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
        CreateMap<ItemTemplateSkillModifierRequest, ItemTemplateSkillModifierEntity>()
            .ForMember(m => m.Id, opt => opt.Ignore())
            .ForMember(m => m.SkillId, opt => opt.MapFrom(x => x.SkillId))
            .ForMember(m => m.Skill, opt => opt.Ignore())
            .ForMember(m => m.ItemTemplateId, opt => opt.Ignore())
            .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
        CreateMap<IdRequest, ItemTemplateSlotEntity>()
            .ForMember(m => m.SlotId, opt => opt.MapFrom(i => i.Id))
            .ForMember(m => m.Slot, opt => opt.Ignore())
            .ForMember(m => m.ItemTemplateId, opt => opt.Ignore())
            .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
        CreateMap<CreateItemTemplateSectionRequest, ItemTemplateSectionEntity>()
            .ForMember(m => m.Id, opt => opt.Ignore())
            .ForMember(m => m.Special, opt => opt.MapFrom(i => string.Join(",", i.Specials)));
        CreateMap<CreateItemTemplateSubCategoryRequest, ItemTemplateSubCategoryEntity>()
            .ForMember(m => m.Id, opt => opt.Ignore())
            .ForMember(m => m.Section, opt => opt.Ignore());
    }
}