using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class ItemTemplateResponse
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string TechName { get; set; }
        public string Source { get; set; }
        public int? SourceUserId { get; set; }
        public string SourceUser { get; set; }
        public JObject Data { get; set; }

        public List<ItemTemplateModifierResponse> Modifiers { get; set; }
        public List<IdResponse> Skills { get; set; }
        [JsonProperty("unskills")]
        public List<IdResponse> UnSkills { get; set; }
        public List<ItemTemplateSkillModifierResponse> SkillModifiers { get; set; }
        public List<ItemTemplateRequirementResponse> Requirements { get; set; }
        public List<ItemSlotResponse> Slots { get; set; }
    }

    public class ItemTemplateModifierResponse
    {
        public string Stat { get; set; }
        public int Value { get; set; }
        public string Type { get; set; }
        public List<string> Special { get; set; }
        [JsonProperty("job")]
        public int? JobId { get; set; }
        [JsonProperty("origin")]
        public int? OriginId { get; set; }
    }

    public class ItemTemplateSkillModifierResponse
    {
        [JsonProperty("skill")]
        public int SkillId { get; set; }
        public short Value { get; set; }
    }

    public class ItemTemplateRequirementResponse
    {
        public string Stat { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}