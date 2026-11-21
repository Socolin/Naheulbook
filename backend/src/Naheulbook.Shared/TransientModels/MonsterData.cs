using Newtonsoft.Json;

namespace Naheulbook.Shared.TransientModels;

[Serializable]
public class MonsterData
{
    public List<MonsterAttack>? Attacks { get; set; }
    public int? Prd { get; set; }
    public int? Esq { get; set; }
    public int Ev { get; set; }
    public int MaxEv { get; set; }
    public int Ea { get; set; }
    public int MaxEa { get; set; }
    public int Pr { get; set; }

    [JsonProperty("pr_magic")]
    public int PrMagic { get; set; }

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
    public List<MonsterSpellBook>? SpellBooks { get; set; }
}

/// <summary>
/// A spell book the monster can cast from (mage, priest of a god…), copied from the monster template.
/// </summary>
[Serializable]
public class MonsterSpellBook
{
    public Guid SpellBookId { get; set; }

    /// <summary>Only the spells up to this level are available. Null: the whole book.</summary>
    public int? MaxLevel { get; set; }
}

[Serializable]
public class MonsterAttack
{
    public string? Name { get; set; }
    public int At { get; set; }
    public string? Dmg { get; set; }
}