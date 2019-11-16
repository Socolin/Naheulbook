using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Requests.Requests
{
    public class CreateCharacterRequest
    {
        public string Name { get; set; } = null!;
        public string Sex { get; set; } = null!;
        public int Money { get; set; }
        public short FatePoint { get; set; }
        public bool IsNpc { get; set; }
        public int? GroupId { get; set; }

        public BasicStats Stats { get; set; } = null!;
        public IDictionary<string, ModifiedStats> ModifiedStat { get; set; } = null!;

        public Guid? JobId { get; set; }
        public Guid OriginId { get; set; }

        public IList<Guid> SkillIds { get; set; } = null!;
        public Guid? SpecialityId { get; set; }
    }

    public class ModifiedStats
    {
        public string Name { get; set; } = null!;
        public IDictionary<string, int> Stats { get; set; } = null!;
    }

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
}