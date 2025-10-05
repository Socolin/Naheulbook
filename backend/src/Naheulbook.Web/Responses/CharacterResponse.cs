using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterResponse
{
    public class BasicStats
    {
        [JsonProperty("AD")]
        public int Ad { get; set; }

        [JsonProperty("COU")]
        public int Cou { get; set; }

        [JsonProperty("CHA")]
        public int Cha { get; set; }

        [JsonProperty("FO")]
        public int Fo { get; set; }

        [JsonProperty("INT")]
        public int Int { get; set; }
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Sex { get; set; } = null!;
    public Guid OriginId { get; set; }
    public bool IsNpc { get; set; }

    public int? Ev { get; set; }
    public int? Ea { get; set; }

    public string? Notes { get; set; }

    public int Level { get; set; }
    public int Experience { get; set; }
    public int FatePoint { get; set; }
    public BasicStats Stats { get; set; } = null!;

    [JsonProperty("statBonusAD")]
    public string? StatBonusAd { get; set; }

    public IList<Guid> JobIds { get; set; } = null!;
    public IList<Guid> SkillIds { get; set; } = null!;

    public CharacterGroupResponse? Group { get; set; }

    public IList<CharacterAptitudeResponse> Aptitudes { get; set; } = null!;
    public IList<ActiveStatsModifier> Modifiers { get; set; } = null!;
    public IList<SpecialityResponse> Specialities { get; set; } = null!;

    public IList<ItemResponse> Items { get; set; } = null!;
    public IList<CharacterGroupInviteResponse> Invites { get; set; } = null!;
}