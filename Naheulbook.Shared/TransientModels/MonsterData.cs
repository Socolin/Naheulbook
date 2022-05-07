using System.Collections.Generic;
using Newtonsoft.Json;

namespace Naheulbook.Shared.TransientModels
{
    public class MonsterData
    {
        public int At { get; set; }
        public int? Prd { get; set; }
        public int? Esq { get; set; }
        public int Ev { get; set; }
        public int MaxEv { get; set; }
        public int Ea { get; set; }
        public int MaxEa { get; set; }
        public int Pr { get; set; }

        [JsonProperty("pr_magic")]
        public int PrMagic { get; set; }

        public string? Dmg { get; set; }
        public int Cou { get; set; }
        public int? Int { get; set; }
        public bool ChercheNoise { get; set; }
        public int Resm { get; set; }
        public int Xp { get; set; }
        public string? Note { get; set; }
        public string? Color { get; set; }
        public int Number { get; set; }
        public string? Sex { get; set; }
        public int? Page { get; set; }
        public List<MonsterAlternativeWeapon>? AlternativeWeapon { get; set; }
    }

    public class MonsterAlternativeWeapon
    {

    }
}