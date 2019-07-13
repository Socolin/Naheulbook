using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Requests.Requests
{
    public class CreateCharacterRequest
    {
        public string Name { get; set; }
        public string Sex { get; set; }
        public int Money { get; set; }
        public int FatePoint { get; set; }
        public bool IsNpc { get; set; }

        public BasicStats Stats { get; set; }
        public IDictionary<string, ModifiedStats> ModifiedStat { get; set; }

        [JsonProperty("job")]
        public int? JobId { get; set; }

        [JsonProperty("origin")]
        public int OriginId { get; set; }

        [JsonProperty("skills")]
        public IList<int> SkillIds { get; set; }

        [JsonProperty("speciality")]
        public int? SpecialityId { get; set; }
    }

    public class ModifiedStats
    {
        public string Name { get; set; }
        public IDictionary<string, int> Stats { get; set; }
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