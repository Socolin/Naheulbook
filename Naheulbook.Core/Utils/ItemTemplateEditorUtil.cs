using System.Collections.Generic;
using System.Linq;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json;

namespace Naheulbook.Core.Utils
{
    public interface IItemTemplateUtil
    {
        void ApplyChangesFromRequest(ItemTemplate itemTemplate, ItemTemplateRequest request);
        IEnumerable<ItemTemplate> FilterItemTemplatesBySource(IEnumerable<ItemTemplate> itemTemplates, int? currentUserId, bool includeCommunityItem);
        ItemTemplateData GetItemTemplateData(ItemTemplate itemTemplate);
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

        public void ApplyChangesFromRequest(ItemTemplate itemTemplate, ItemTemplateRequest request)
        {
            itemTemplate.Name = request.Name;
            itemTemplate.TechName = request.TechName;
            itemTemplate.Source = request.Source;
            itemTemplate.SubCategoryId = request.SubCategoryId;
            itemTemplate.Data = JsonConvert.SerializeObject(request.Data, Formatting.None);

            itemTemplate.CleanName = _stringCleanupUtil.RemoveAccents(request.Name);

            itemTemplate.Slots = request.Slots?.Select(x => new ItemTemplateSlot {SlotId = x.Id}).ToList() ?? new List<ItemTemplateSlot>();

            itemTemplate.Requirements = request.Requirements?.Select(x => new ItemTemplateRequirement
            {
                StatName = x.Stat,
                MaxValue = x.Max,
                MinValue = x.Min
            }).ToList() ?? new List<ItemTemplateRequirement>();

            itemTemplate.Modifiers = request.Modifiers?.Select(x => new ItemTemplateModifier
            {
                StatName = x.Stat,
                RequireJobId = x.Job,
                RequireOriginId = x.Origin,
                Value = x.Value,
                Type = x.Type,
                Special = x.Special == null ? "" : string.Join(',', x.Special)
            }).ToList() ?? new List<ItemTemplateModifier>();

            itemTemplate.Skills = request.SkillIds?.Select(skillId => new ItemTemplateSkill
            {
                SkillId = skillId
            }).ToList() ?? new List<ItemTemplateSkill>();

            itemTemplate.UnSkills = request.UnSkillIds?.Select(skillId => new ItemTemplateUnSkill
            {
                SkillId = skillId
            }).ToList() ?? new List<ItemTemplateUnSkill>();

            itemTemplate.SkillModifiers = request.SkillModifiers?.Select(x => new ItemTemplateSkillModifier
            {
                SkillId = x.SkillId,
                Value = x.Value
            }).ToList() ?? new List<ItemTemplateSkillModifier>();
        }

        public IEnumerable<ItemTemplate> FilterItemTemplatesBySource(IEnumerable<ItemTemplate> itemTemplates, int? currentUserId, bool includeCommunityItem)
        {
            return itemTemplates.Where(x => x.Source == ItemTemplate.OfficialSourceValue
                                            || (x.Source == ItemTemplate.CommunitySourceValue && includeCommunityItem)
                                            || (x.Source == ItemTemplate.PrivateSourceValue && x.SourceUserId == currentUserId)
            );
        }

        public ItemTemplateData GetItemTemplateData(ItemTemplate itemTemplate)
        {
            return _jsonUtil.Deserialize<ItemTemplateData>(itemTemplate.Data) ?? new ItemTemplateData();
        }
    }
}