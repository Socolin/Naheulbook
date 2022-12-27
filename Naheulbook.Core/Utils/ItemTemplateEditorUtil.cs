using System.Collections.Generic;
using System.Linq;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json;

namespace Naheulbook.Core.Utils;

public interface IItemTemplateUtil
{
    void ApplyChangesFromRequest(ItemTemplateEntity itemTemplate, ItemTemplateRequest request);
    IEnumerable<ItemTemplateEntity> FilterItemTemplatesBySource(IEnumerable<ItemTemplateEntity> itemTemplates, int? currentUserId, bool includeCommunityItem);
    ItemTemplateData GetItemTemplateData(ItemTemplateEntity itemTemplate);
}

public class ItemTemplateUtil : IItemTemplateUtil
{
    private readonly IStringCleanupUtil _stringCleanupUtil;
    private readonly IJsonUtil _jsonUtil;

    public ItemTemplateUtil(
        IStringCleanupUtil stringCleanupUtil,
        IJsonUtil jsonUtil
    )
    {
        _stringCleanupUtil = stringCleanupUtil;
        _jsonUtil = jsonUtil;
    }

    public void ApplyChangesFromRequest(ItemTemplateEntity itemTemplate, ItemTemplateRequest request)
    {
        itemTemplate.Name = request.Name;
        itemTemplate.TechName = request.TechName;
        itemTemplate.Source = request.Source;
        itemTemplate.SubCategoryId = request.SubCategoryId;
        itemTemplate.Data = JsonConvert.SerializeObject(request.Data, Formatting.None);

        itemTemplate.CleanName = _stringCleanupUtil.RemoveAccents(request.Name);

        itemTemplate.Slots = request.Slots?.Select(x => new ItemTemplateSlotEntity {SlotId = x.Id}).ToList() ?? new List<ItemTemplateSlotEntity>();

        itemTemplate.Requirements = request.Requirements?.Select(x => new ItemTemplateRequirementEntity
        {
            StatName = x.Stat,
            MaxValue = x.Max,
            MinValue = x.Min
        }).ToList() ?? new List<ItemTemplateRequirementEntity>();

        itemTemplate.Modifiers = request.Modifiers?.Select(x => new ItemTemplateModifierEntity
        {
            StatName = x.Stat,
            RequiredJobId = x.JobId,
            RequiredOriginId = x.OriginId,
            Value = x.Value,
            Type = x.Type,
            Special = x.Special == null ? "" : string.Join(',', x.Special)
        }).ToList() ?? new List<ItemTemplateModifierEntity>();

        itemTemplate.Skills = request.SkillIds?.Select(skillId => new ItemTemplateSkillEntity
        {
            SkillId = skillId
        }).ToList() ?? new List<ItemTemplateSkillEntity>();

        itemTemplate.UnSkills = request.UnSkillIds?.Select(skillId => new ItemTemplateUnSkillEntity
        {
            SkillId = skillId
        }).ToList() ?? new List<ItemTemplateUnSkillEntity>();

        itemTemplate.SkillModifiers = request.SkillModifiers?.Select(x => new ItemTemplateSkillModifierEntity
        {
            SkillId = x.SkillId,
            Value = x.Value
        }).ToList() ?? new List<ItemTemplateSkillModifierEntity>();
    }

    public IEnumerable<ItemTemplateEntity> FilterItemTemplatesBySource(IEnumerable<ItemTemplateEntity> itemTemplates, int? currentUserId, bool includeCommunityItem)
    {
        return itemTemplates.Where(x => x.Source == ItemTemplateEntity.OfficialSourceValue
                                        || (x.Source == ItemTemplateEntity.CommunitySourceValue && includeCommunityItem)
                                        || (x.Source == ItemTemplateEntity.PrivateSourceValue && x.SourceUserId == currentUserId)
        );
    }

    public ItemTemplateData GetItemTemplateData(ItemTemplateEntity itemTemplate)
    {
        return _jsonUtil.Deserialize<ItemTemplateData>(itemTemplate.Data) ?? new ItemTemplateData();
    }
}