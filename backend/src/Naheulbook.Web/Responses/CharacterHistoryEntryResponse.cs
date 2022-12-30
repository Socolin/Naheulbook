using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterHistoryEntryResponse : IHistoryEntryResponse
{
    public int Id { get; set; }
    public string Action { get; set; } = null!;
    public JObject? Data { get; set; }
    public string Date { get; set; } = null!;
    public bool Gm { get; set; }
    public string? Info { get; set; }
    public bool IsGroup => false;
    public ItemHistoryResponse? Item { get; set; }
    public ModifierHistoryResponse? Modifier { get; set; }
    public EffectHistoryResponse? Effect { get; set; }

    public class ItemHistoryResponse
    {
        public required string? Name { get; set; }
    }

    public class ModifierHistoryResponse
    {
        public required string Name { get; set; }
    }

    public class EffectHistoryResponse
    {
        public required string Name { get; set; }
    }
}