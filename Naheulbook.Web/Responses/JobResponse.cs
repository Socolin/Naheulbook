using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class JobResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [JsonProperty("internalname")]
        public string? InternalName { get; set; }

        [JsonProperty("informations")]
        public string? Information { get; set; }

        public string? PlayerDescription { get; set; }
        public string? PlayerSummary { get; set; }
        public long? MaxLoad { get; set; }

        [JsonProperty("maxArmorPR")]
        public short? MaxArmorPr { get; set; }

        public bool? IsMagic { get; set; }
        public short? BaseEv { get; set; }
        public double? FactorEv { get; set; }
        public short? BonusEv { get; set; }
        public short? BaseEa { get; set; }
        public short? DiceEaLevelUp { get; set; }

        [JsonProperty("baseAT")]
        public short? BaseAt { get; set; }

        [JsonProperty("basePRD")]
        public short? BasePrd { get; set; }

        public long? ParentJobId { get; set; }
        public List<FlagResponse>? Flags { get; set; }
        public List<int> SkillIds { get; set; } = null!;
        public List<int> AvailableSkillIds { get; set; } = null!;
        public List<NamedIdResponse> OriginsWhitelist { get; set; } = null!;
        public List<NamedIdResponse> OriginsBlacklist { get; set; } = null!;
        public List<DescribedFlagResponse> Bonuses { get; set; } = null!;
        public List<StatRequirementResponse> Requirements { get; set; } = null!;

        [JsonProperty("restricts")]
        public List<DescribedFlagResponse> Restrictions { get; set; } = null!;

        public List<SpecialityResponse> Specialities { get; set; } = null!;
    }
}