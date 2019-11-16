using System;
using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Web.Responses
{
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
        public int OriginId { get; set; }
        public bool IsNpc { get; set; }

        public int? Ev { get; set; }
        public int? Ea { get; set; }

        public int Level { get; set; }
        public int Experience { get; set; }
        public int FatePoint { get; set; }
        public BasicStats Stats { get; set; } = null!;

        [JsonProperty("statBonusAD")]
        public string? StatBonusAd { get; set; }

        public IList<int> JobIds { get; set; } = null!;
        public IList<Guid> SkillIds { get; set; } = null!;

        public NamedIdResponse? Group { get; set; }

        public IList<ActiveStatsModifier> Modifiers { get; set; } = null!;
        public IList<SpecialityResponse> Specialities { get; set; } = null!;

        public IList<ItemResponse> Items { get; set; } = null!;
        public IList<CharacterGroupInviteResponse> Invites { get; set; } = null!;
    }
}