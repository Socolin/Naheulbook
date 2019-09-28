using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class ItemTemplateResponse
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? TechName { get; set; }
        public string Source { get; set; } = null!;
        public int? SourceUserId { get; set; }
        public string? SourceUser { get; set; }
        public JObject Data { get; set; } = null!;

        public List<ItemTemplateModifierResponse> Modifiers { get; set; } = null!;
        public List<IdResponse> Skills { get; set; } = null!;

        [JsonProperty("unskills")]
        public List<IdResponse> UnSkills { get; set; } = null!;

        public List<ItemTemplateSkillModifierResponse> SkillModifiers { get; set; } = null!;
        public List<ItemTemplateRequirementResponse> Requirements { get; set; } = null!;
        public List<ItemSlotResponse> Slots { get; set; } = null!;
    }

    public class ItemTemplateModifierResponse
    {
        public string Stat { get; set; } = null!;
        public int Value { get; set; }
        public string Type { get; set; } = null!;
        public List<string>? Special { get; set; }

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
        public string Stat { get; set; } = null!;
        public int Min { get; set; }
        public int Max { get; set; }
    }
}