using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Features.Item;

public interface IItemTemplateMapper
{
    ItemTemplateResponse ToResponse(ItemTemplateEntity itemTemplate);
    ItemTemplateModifierResponse ToResponse(ItemTemplateModifierEntity itemTemplateModifier);
    ItemTemplateRequirementResponse ToResponse(ItemTemplateRequirementEntity itemTemplateRequirement);
    ItemTemplateSkillModifierResponse ToResponse(ItemTemplateSkillModifierEntity itemTemplateSkillModifier);
    ItemSlotResponse ToResponse(SlotEntity slot);
    ItemTemplateSectionResponse ToResponse(ItemTemplateSectionEntity itemTemplateSection);
    ItemTemplateSubCategoryResponse ToResponse(ItemTemplateSubCategoryEntity itemTemplateSubCategory);
    ItemTypeResponse ToResponse(ItemTypeEntity itemType);

    ItemTemplateEntity ToEntity(ItemTemplateRequest request);
}

public class ItemTemplateMapper : IItemTemplateMapper
{
    public ItemTemplateResponse ToResponse(ItemTemplateEntity itemTemplate)
    {
        return new ItemTemplateResponse
        {
            Id = itemTemplate.Id,
            SubCategoryId = itemTemplate.SubCategoryId,
            Name = itemTemplate.Name,
            TechName = itemTemplate.TechName,
            Source = itemTemplate.Source,
            SourceUserId = itemTemplate.SourceUserId,
            SourceUser = itemTemplate.SourceUserNameCache,
            Data = MapperHelpers.FromJson<JObject>(itemTemplate.Data) ?? new JObject(),
            Modifiers = itemTemplate.Modifiers.Select(ToResponse).ToList(),
            SkillIds = itemTemplate.Skills.Select(s => s.SkillId).ToList(),
            UnSkillIds = itemTemplate.UnSkills.Select(s => s.SkillId).ToList(),
            SkillModifiers = itemTemplate.SkillModifiers.Select(ToResponse).ToList(),
            Requirements = itemTemplate.Requirements.Select(ToResponse).ToList(),
            Slots = itemTemplate.Slots.Select(s => ToResponse(s.Slot)).ToList(),
        };
    }

    public ItemTemplateModifierResponse ToResponse(ItemTemplateModifierEntity itemTemplateModifier)
    {
        return new ItemTemplateModifierResponse
        {
            Stat = itemTemplateModifier.StatName,
            Value = itemTemplateModifier.Value,
            Type = itemTemplateModifier.Type,
            Special = MapperHelpers.FromCommaSeparatedList(itemTemplateModifier.Special) ?? [],
            JobId = itemTemplateModifier.RequiredJobId,
            OriginId = itemTemplateModifier.RequiredOriginId,
        };
    }

    public ItemTemplateRequirementResponse ToResponse(ItemTemplateRequirementEntity itemTemplateRequirement)
    {
        return new ItemTemplateRequirementResponse
        {
            Stat = itemTemplateRequirement.StatName,
            Min = itemTemplateRequirement.MinValue ?? 0,
            Max = itemTemplateRequirement.MaxValue ?? 0,
        };
    }

    public ItemTemplateSkillModifierResponse ToResponse(ItemTemplateSkillModifierEntity itemTemplateSkillModifier)
    {
        return new ItemTemplateSkillModifierResponse
        {
            SkillId = itemTemplateSkillModifier.SkillId,
            Value = itemTemplateSkillModifier.Value,
        };
    }

    public ItemSlotResponse ToResponse(SlotEntity slot)
    {
        return new ItemSlotResponse
        {
            Id = slot.Id,
            Name = slot.Name,
            TechName = slot.TechName,
        };
    }

    public ItemTemplateSectionResponse ToResponse(ItemTemplateSectionEntity itemTemplateSection)
    {
        return new ItemTemplateSectionResponse
        {
            Id = itemTemplateSection.Id,
            Name = itemTemplateSection.Name,
            Note = itemTemplateSection.Note,
            Icon = itemTemplateSection.Icon,
            Specials = MapperHelpers.FromCommaSeparatedList(itemTemplateSection.Special) ?? [],
            SubCategories = itemTemplateSection.SubCategories.OrderBy(x => x.Id).Select(ToResponse).ToList(),
        };
    }

    public ItemTemplateSubCategoryResponse ToResponse(ItemTemplateSubCategoryEntity itemTemplateSubCategory)
    {
        return new ItemTemplateSubCategoryResponse
        {
            Id = itemTemplateSubCategory.Id,
            SectionId = itemTemplateSubCategory.SectionId,
            Name = itemTemplateSubCategory.Name,
            Description = itemTemplateSubCategory.Description,
            TechName = itemTemplateSubCategory.TechName,
            Note = itemTemplateSubCategory.Note,
        };
    }

    public ItemTypeResponse ToResponse(ItemTypeEntity itemType)
    {
        return new ItemTypeResponse
        {
            Id = itemType.Id,
            DisplayName = itemType.DisplayName,
            TechName = itemType.TechName,
        };
    }

    public ItemTemplateEntity ToEntity(ItemTemplateRequest request)
    {
        return new ItemTemplateEntity
        {
            Name = request.Name,
            CleanName = StringCleanupHelper.RemoveAccents(request.Name),
            TechName = request.TechName,
            Source = request.Source,
            SubCategoryId = request.SubCategoryId,
            Data = request.Data.ToString(Formatting.None),
            Modifiers = request.Modifiers.Select(m => new ItemTemplateModifierEntity
            {
                StatName = m.Stat,
                Value = m.Value,
                Type = m.Type,
                Special = m.Special == null ? null : string.Join(',', m.Special),
                RequiredJobId = m.JobId,
                RequiredOriginId = m.OriginId,
            }).ToList(),
            Requirements = request.Requirements.Select(r => new ItemTemplateRequirementEntity
            {
                StatName = r.Stat,
                MinValue = r.Min,
                MaxValue = r.Max,
            }).ToList(),
            SkillModifiers = request.SkillModifiers.Select(s => new ItemTemplateSkillModifierEntity
            {
                SkillId = s.SkillId,
                Value = s.Value,
            }).ToList(),
            Slots = request.Slots.Select(s => new ItemTemplateSlotEntity {SlotId = s.Id}).ToList(),
            Skills = request.SkillIds.Select(skillId => new ItemTemplateSkillEntity {SkillId = skillId}).ToList(),
            UnSkills = request.UnSkillIds.Select(skillId => new ItemTemplateUnSkillEntity {SkillId = skillId}).ToList(),
        };
    }
}
