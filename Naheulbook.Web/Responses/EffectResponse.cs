using System.Collections.Generic;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class EffectResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("durationType")]
        public string DurationType { get; set; }

        [JsonProperty("duration")]
        public object Duration { get; set; }

        [JsonProperty("combatCount")]
        public object CombatCount { get; set; }

        [JsonProperty("lapCount")]
        public object LapCount { get; set; }

        [JsonProperty("timeDuration")]
        public object TimeDuration { get; set; }

        [JsonProperty("dice")]
        public long Dice { get; set; }

        [JsonProperty("categoryId")]
        public long CategoryId { get; set; }

        [JsonProperty("modifiers")]
        public IList<StatModifierResponse> Modifiers { get; set; }
    }


    public class EffectTypeResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("categories")]
        public IList<EffectCategoryResponse> Categories { get; set; }
    }

    public class EffectCategoryResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("diceCount")]
        public short DiceCount { get; set; }

        [JsonProperty("diceSize")]
        public short DiceSize { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("typeId")]
        public long TypeId { get; set; }
    }
}