using AutoMapper;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Newtonsoft.Json;

namespace Naheulbook.Core.Mappers
{
    public class RequestToEntityProfile : Profile
    {
        public RequestToEntityProfile()
        {
            CreateMap<ItemTemplateRequest, ItemTemplate>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.CleanName, opt => opt.MapFrom(i => StringCleanupHelper.RemoveAccents(i.Name)))
                .ForMember(m => m.Category, opt => opt.Ignore())
                .ForMember(m => m.SourceUserId, opt => opt.Ignore())
                .ForMember(m => m.SourceUser, opt => opt.Ignore())
                .ForMember(m => m.Data, opt => opt.MapFrom(i => i.Data.ToString(Formatting.None)));
            CreateMap<ItemTemplateModifierRequest, ItemTemplateModifier>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.RequireJobId, opt => opt.MapFrom(im => im.Job))
                .ForMember(m => m.RequireOriginId, opt => opt.MapFrom(im => im.Origin))
                .ForMember(m => m.Special, opt => opt.MapFrom(im => JsonConvert.SerializeObject(im.Special)))
                .ForMember(m => m.StatName, opt => opt.MapFrom(im => im.Stat))
                .ForMember(m => m.Stat, opt => opt.Ignore())
                .ForMember(m => m.RequireJob, opt => opt.Ignore())
                .ForMember(m => m.RequireOrigin, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
            CreateMap<ItemTemplateRequirementRequest, ItemTemplateRequirement>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.StatName, opt => opt.MapFrom(ir => ir.Stat))
                .ForMember(m => m.Stat, opt => opt.Ignore())
                .ForMember(m => m.MinValue, opt => opt.MapFrom(ir => ir.Min))
                .ForMember(m => m.MaxValue, opt => opt.MapFrom(ir => ir.Max))
                .ForMember(m => m.ItemTemplateId, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
            CreateMap<ItemTemplateSkillModifierRequest, ItemTemplateSkillModifier>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.SkillId, opt => opt.MapFrom(x => x.Skill))
                .ForMember(m => m.Skill, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplateId, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
            CreateMap<IdRequest, ItemTemplateSlot>()
                .ForMember(m => m.SlotId, opt => opt.MapFrom(i => i.Id))
                .ForMember(m => m.Slot, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplateId, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
            CreateMap<IdRequest, ItemTemplateSkill>()
                .ForMember(m => m.SkillId, opt => opt.MapFrom(i => i.Id))
                .ForMember(m => m.Skill, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplateId, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
            CreateMap<IdRequest, ItemTemplateUnSkill>()
                .ForMember(m => m.SkillId, opt => opt.MapFrom(i => i.Id))
                .ForMember(m => m.Skill, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplateId, opt => opt.Ignore())
                .ForMember(m => m.ItemTemplate, opt => opt.Ignore());
            CreateMap<CreateItemTemplateSectionRequest, ItemTemplateSection>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.Special, opt => opt.MapFrom(i => string.Join(",", i.Specials)));
            CreateMap<CreateItemTemplateCategoryRequest, ItemTemplateCategory>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.Section, opt => opt.Ignore());
        }
    }
}