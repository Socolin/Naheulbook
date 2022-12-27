using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global

namespace Naheulbook.Shared.TransientModels;

public interface IReadOnlyItemData
{
    string? Name { get; }
    string? Description { get; }
    int? Quantity { get; }
    JToken? Icon { get; }
    int? Charge { get; }
    int? Ug { get; }
    int? Equipped { get; }
    int? ReadCount { get; }
    bool? NotIdentified { get; }
    bool? IgnoreRestrictions { get; }
    JToken? Lifetime { get; }
    bool? ShownToGm { get; }
}

public class ItemData : IReadOnlyItemData
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public JToken? Icon { get; set; }
    public int? Charge { get; set; }
    public int? Ug { get; set; }

    [JsonProperty("equiped")]
    public int? Equipped { get; set; }

    public int? ReadCount { get; set; }
    public bool? NotIdentified { get; set; }
    public bool? IgnoreRestrictions { get; set; }
    public JToken? Lifetime { get; set; }
    public bool? ShownToGm { get; set; }
}