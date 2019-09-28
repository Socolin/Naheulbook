using System.Collections.Generic;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class EffectResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("durationType")]
        public string DurationType { get; set; } = null!;

        [JsonProperty("duration")]
        public string? Duration { get; set; }

        [JsonProperty("combatCount")]
        public int? CombatCount { get; set; }

        [JsonProperty("lapCount")]
        public int? LapCount { get; set; }

        [JsonProperty("timeDuration")]
        public int? TimeDuration { get; set; }

        [JsonProperty("dice")]
        public int? Dice { get; set; }

        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("modifiers")]
        public IList<StatModifierResponse> Modifiers { get; set; } = null!;
    }

    public class EffectTypeResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("categories")]
        public IList<EffectCategoryResponse> Categories { get; set; } = null!;
    }

    public class EffectCategoryResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("diceCount")]
        public short DiceCount { get; set; }

        [JsonProperty("diceSize")]
        public short DiceSize { get; set; }

        [JsonProperty("note")]
        public string? Note { get; set; }

        [JsonProperty("typeId")]
        public int TypeId { get; set; }
    }
}