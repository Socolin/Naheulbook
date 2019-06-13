using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests
{
    public class ItemTemplateRequest
    {
        public string Source { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string TechName { get; set; }
        public List<ItemTemplateModifierRequest> Modifiers { get; set; }
        public List<IdRequest> Skills { get; set; }
        public List<IdRequest> UnSkills { get; set; }
        public List<ItemTemplateSkillModifierRequest> SkillModifiers { get; set; }
        public List<ItemTemplateRequirementRequest> Requirements { get; set; }
        public List<IdRequest> Slots { get; set; }
        public JObject Data { get; set; }
    }

    public class ItemTemplateModifierRequest
    {
        public string Stat { get; set; }
        public int Value { get; set; }
        public string Type { get; set; }
        public List<string> Special { get; set; }
        public int? Job { get; set; }
        public int? Origin { get; set; }
    }

    public class ItemTemplateSkillModifierRequest
    {
        public int Skill { get; set; }
        public short Value { get; set; }
    }

    public class ItemTemplateRequirementRequest
    {
        public string Stat { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}