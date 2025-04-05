using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Shared.TransientModels;

public class ItemTemplateData
{
    public List<NhbkAction>? Actions { get; set; }
    public string? AvailableLocation { get; set; }
    public int? BonusDamage { get; set; }
    public bool? BruteWeapon { get; set; }
    public int? Charge { get; set; }
    public bool? Container { get; set; }
    public int? DamageDice { get; set; }
    public JToken? DamageType { get; set; }
    public string? Description { get; set; }
    public int? DiceDrop { get; set; }
    public string? Duration { get; set; }
    public string? Enchantment { get; set; }
    public string? God { get; set; }
    public ItemTemplateGunData? Gun { get; set; }
    public IconDescription? Icon { get; set; }
    public bool? IsCurrency { get; set; }
    public string[]? ItemTypes { get; set; }
    public ItemTemplateInstrumentData? Instrument { get; set; }
    public Durable? Lifetime { get; set; }
    public int? MagicProtection { get; set; }
    public string? Note { get; set; }
    public string? NotIdentifiedName { get; set; }
    public string? Origin { get; set; }
    public int? Price { get; set; }
    public int? Protection { get; set; }
    public JToken? ProtectionAgainstMagic { get; set; }
    public JToken? ProtectionAgainstType { get; set; }
    public bool? Quantifiable { get; set; }
    public string? RarityIndicator { get; set; }
    public bool? Relic { get; set; }
    public int? RequireLevel { get; set; }
    public int? Rupture { get; set; }
    public string? Sex { get; set; }
    public bool? SkillBook { get; set; }
    public string? Space { get; set; }
    public bool? Throwable { get; set; }

    [JsonProperty("UseUG")]
    public bool? UseUg { get; set; }

    public int? Weight { get; set; }
}