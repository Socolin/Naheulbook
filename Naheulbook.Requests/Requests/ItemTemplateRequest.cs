using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests
{
    public class ItemTemplateRequest
    {
        public string Source { get; set; } = null!;
        public int SubCategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? TechName { get; set; }
        public List<ItemTemplateModifierRequest> Modifiers { get; set; } = new List<ItemTemplateModifierRequest>();
        public List<IdRequest> Skills { get; set; } = new List<IdRequest>();
        [JsonProperty("unskills")]
        public List<IdRequest> UnSkills { get; set; } = new List<IdRequest>();
        public List<ItemTemplateSkillModifierRequest> SkillModifiers { get; set; } = new List<ItemTemplateSkillModifierRequest>();
        public List<ItemTemplateRequirementRequest> Requirements { get; set; } = new List<ItemTemplateRequirementRequest>();
        public List<IdRequest> Slots { get; set; } = new List<IdRequest>();
        public JObject Data { get; set; } = null!;
    }

    public class ItemTemplateModifierRequest
    {
        public string Stat { get; set; } = null!;
        public int Value { get; set; }
        public string Type { get; set; } = null!;
        public List<string>? Special { get; set; }
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
        public string Stat { get; set; } = null!;
        public int Min { get; set; }
        public int Max { get; set; }
    }
}