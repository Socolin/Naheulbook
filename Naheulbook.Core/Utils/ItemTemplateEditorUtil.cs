using System.Collections.Generic;
using System.Linq;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Newtonsoft.Json;

namespace Naheulbook.Core.Utils
{
    public interface IItemTemplateUtil
    {
        void ApplyChangesFromRequest(ItemTemplate itemTemplate, ItemTemplateRequest request);
        IEnumerable<ItemTemplate> FilterItemTemplatesBySource(IEnumerable<ItemTemplate> itemTemplates, int? currentUserId, bool includeCommunityItem);
    }

    public class ItemTemplateUtil : IItemTemplateUtil
    {
        private readonly IStringCleanupUtil _stringCleanupUtil;

        public ItemTemplateUtil(IStringCleanupUtil stringCleanupUtil)
        {
            _stringCleanupUtil = stringCleanupUtil;
        }

        public void ApplyChangesFromRequest(ItemTemplate itemTemplate, ItemTemplateRequest request)
        {
            itemTemplate.Name = request.Name;
            itemTemplate.TechName = request.TechName;
            itemTemplate.Source = request.Source;
            itemTemplate.CategoryId = request.CategoryId;
            itemTemplate.Data = JsonConvert.SerializeObject(request.Data, Formatting.None);

            itemTemplate.CleanName = _stringCleanupUtil.RemoveAccents(request.Name);

            itemTemplate.Slots = request.Slots?.Select(x => new ItemTemplateSlot {SlotId = x.Id}).ToList();

            itemTemplate.Requirements = request.Requirements?.Select(x => new ItemTemplateRequirement
            {
                StatName = x.Stat,
                MaxValue = x.Max,
                MinValue = x.Min
            }).ToList();

            itemTemplate.Modifiers = request.Modifiers?.Select(x => new ItemTemplateModifier
            {
                StatName = x.Stat,
                RequireJobId = x.Job,
                RequireOriginId = x.Origin,
                Value = x.Value,
                Type = x.Type,
                Special = x.Special == null ? "" : string.Join(',', x.Special)
            }).ToList();


            itemTemplate.Skills = request.Skills?.Select(x => new ItemTemplateSkill
            {
                SkillId = x.Id
            }).ToList();

            itemTemplate.UnSkills = request.UnSkills?.Select(x => new ItemTemplateUnSkill
            {
                SkillId = x.Id
            }).ToList();

            itemTemplate.SkillModifiers = request.SkillModifiers?.Select(x => new ItemTemplateSkillModifier()
            {
                SkillId = x.Skill,
                Value = x.Value
            }).ToList();
        }

        public IEnumerable<ItemTemplate> FilterItemTemplatesBySource(IEnumerable<ItemTemplate> itemTemplates, int? currentUserId, bool includeCommunityItem)
        {
            return itemTemplates.Where(x => x.Source == ItemTemplate.OfficialSourceValue
                                            || (x.Source == ItemTemplate.CommunitySourceValue && includeCommunityItem)
                                            || (x.Source == ItemTemplate.PrivateSourceValue && x.SourceUserId == currentUserId)
            );
        }
    }
}