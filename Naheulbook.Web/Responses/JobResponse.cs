using System.Collections.Generic;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class JobResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("internalname")]
        public string InternalName { get; set; }
        [JsonProperty("informations")]
        public string Information { get; set; }
        public string PlayerDescription { get; set; }
        public string PlayerSummary { get; set; }
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
        public List<FlagResponse> Flags { get; set; }
        public List<int> SkillIds { get; set; }
        public List<int> AvailableSkillIds { get; set; }
        public List<NamedIdResponse> OriginsWhitelist { get; set; }
        public List<NamedIdResponse> OriginsBlacklist { get; set; }
        public List<DescribedFlagResponse> Bonuses { get; set; }
        public List<StatRequirementResponse> Requirements { get; set; }
        [JsonProperty("restricts")]
        public List<DescribedFlagResponse> Restrictions { get; set; }
        public List<SpecialityResponse> Specialities { get; set; }
    }
}