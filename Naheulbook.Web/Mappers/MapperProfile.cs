using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Naheulbook.Data.Models;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Effect, EffectResponse>();
            CreateMap<EffectType, EffectTypeResponse>();
            CreateMap<EffectCategory, EffectCategoryResponse>();
            CreateMap<EffectModifier, StatModifierResponse>()
                .ForMember(m => m.Stat, opt => opt.MapFrom(se => se.StatName))
                .ForMember(m => m.Special, opt => opt.Ignore());

            CreateMap<Job, JobResponse>()
                .ForMember(m => m.Requirements, opt => opt.MapFrom(j => j.Requirements.OrderBy(r => r.Id)))
                .ForMember(m => m.OriginsWhitelist, opt => opt.MapFrom(j => j.OriginWhitelist.OrderBy(w => w.Id)))
                .ForMember(m => m.OriginsBlacklist, opt => opt.MapFrom(j => j.OriginBlacklist.OrderBy(b => b.Id)))
                .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)))
                .ForMember(m => m.AvailableSkillIds, opt => opt.MapFrom(j => j.Skills.Where(s => !s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId)))
                .ForMember(m => m.SkillIds, opt => opt.MapFrom(j => j.Skills.Where(s => s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId)));
            CreateMap<JobBonus, DescribedFlagResponse>()
                .ForMember(m => m.Flags, opt => opt.MapFrom(b => MapperHelpers.FromJson<List<FlagResponse>>(b.Flags)));
            CreateMap<JobOriginBlacklist, NamedIdResponse>()
                .ForMember(m => m.Id, opt => opt.MapFrom(b => b.Origin.Id))
                .ForMember(m => m.Name, opt => opt.MapFrom(b => b.Origin.Name));
            CreateMap<JobOriginWhitelist, NamedIdResponse>()
                .ForMember(m => m.Id, opt => opt.MapFrom(w => w.Origin.Id))
                .ForMember(m => m.Name, opt => opt.MapFrom(w => w.Origin.Name));
            CreateMap<JobRequirement, StatRequirementResponse>()
                .ForMember(m => m.Stat, opt => opt.MapFrom(r => r.StatName))
                .ForMember(m => m.Min, opt => opt.MapFrom(r => r.MinValue))
                .ForMember(m => m.Max, opt => opt.MapFrom(r => r.MaxValue));
            CreateMap<JobRestrict, DescribedFlagResponse>()
                .ForMember(m => m.Description, opt => opt.MapFrom(r => r.Text))
                .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)));

            CreateMap<Origin, OriginResponse>()
                .ForMember(m => m.Requirements, opt => opt.MapFrom(o => o.Requirements.OrderBy(r => r.Id)))
                .ForMember(m => m.Flags, opt => opt.MapFrom(o => MapperHelpers.FromJson<List<FlagResponse>>(o.Flags)))
                .ForMember(m => m.AvailableSkillIds, opt => opt.MapFrom(o => o.Skills.Where(s => !s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId)))
                .ForMember(m => m.SkillIds, opt => opt.MapFrom(o => o.Skills.Where(s => s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId)));
            CreateMap<OriginInfo, OriginInformationResponse>();
            CreateMap<OriginRequirement, OriginRequirementResponse>()
                .ForMember(m => m.Stat, opt => opt.MapFrom(r => r.StatName))
                .ForMember(m => m.Min, opt => opt.MapFrom(r => r.MinValue))
                .ForMember(m => m.Max, opt => opt.MapFrom(r => r.MaxValue));
            CreateMap<OriginBonus, DescribedFlagResponse>()
                .ForMember(m => m.Flags, opt => opt.MapFrom(b => MapperHelpers.FromJson<List<FlagResponse>>(b.Flags)));
            CreateMap<OriginRestrict, DescribedFlagResponse>()
                .ForMember(m => m.Description, opt => opt.MapFrom(r => r.Text))
                .ForMember(m => m.Flags, opt => opt.MapFrom(r => MapperHelpers.FromJson<List<FlagResponse>>(r.Flags)));

            CreateMap<SkillEffect, SkillEffectResponse>()
                .ForMember(m => m.Type, opt => opt.UseValue("Add"))
                .ForMember(m => m.Stat, opt => opt.MapFrom(s => s.StatName));
            CreateMap<Skill, SkillResponse>()
                .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)))
                .ForMember(m => m.Effects, opt => opt.MapFrom(s => s.SkillEffects))
                .ForMember(m => m.Stat, opt => opt.MapFrom(s => MapperHelpers.FromCommaSeparatedStringArray(s.Stat)));

            CreateMap<Speciality, SpecialityResponse>()
                .ForMember(m => m.Modifiers, opt => opt.MapFrom(s => s.Modifiers.OrderBy(m => m.Id)))
                .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)))
                .ForMember(m => m.Specials, opt => opt.MapFrom(s => s.Specials.OrderBy(p => p.Id)));
            CreateMap<SpecialitySpecial, SpecialitySpecialResponse>()
                .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)));
            CreateMap<SpecialityModifier, StatModifierResponse>()
                .ForMember(m => m.Type, opt => opt.UseValue("Add"))
                .ForMember(m => m.Special, opt => opt.Ignore());
        }
    }
}