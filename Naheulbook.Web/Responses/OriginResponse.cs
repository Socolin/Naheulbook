using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class OriginResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? PlayerDescription { get; set; }
        public string? PlayerSummary { get; set; }
        public long? MaxLoad { get; set; }
        [JsonProperty("maxArmorPR")]
        public short? MaxArmorPr { get; set; }
        public string? Advantage { get; set; }
        [JsonProperty("baseEV")]
        public short BaseEv { get; set; }
        [JsonProperty("baseEA")]
        public short? BaseEa { get; set; }
        [JsonProperty("bonusAT")]
        public short? BonusAt { get; set; }
        [JsonProperty("bonusPRD")]
        public short? BonusPrd { get; set; }
        [JsonProperty("diceEVLevelUp")]
        public int DiceEvLevelUp { get; set; }
        public string? Size { get; set; }
        public List<FlagResponse>? Flags { get; set; }
        public short? SpeedModifier { get; set; }
        public List<int> SkillIds { get; set; } = null!;
        public List<int> AvailableSkillIds { get; set; } = null!;
        [JsonProperty("infos")]
        public IEnumerable<OriginInformationResponse> Information { get; set; } = null!;
        public ICollection<DescribedFlagResponse> Bonuses { get; set; } = null!;
        public ICollection<OriginRequirementResponse> Requirements { get; set; } = null!;
        [JsonProperty("restricts")]
        public ICollection<DescribedFlagResponse> Restrictions { get; set; } = null!;
    }

    public class OriginRequirementResponse
    {
        public string Stat { get; set; } = null!;
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

    public class OriginInformationResponse
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}