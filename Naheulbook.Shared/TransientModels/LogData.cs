using Newtonsoft.Json.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Shared.TransientModels
{
    public class ItemLogData
    {
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public JToken? Icon { get; set; }
        public int? Ug { get; set; }
    }

    public class ItemQuantityChangeLogData : ItemLogData
    {
        public int? OldValue { get; set; }
        public int? NewValue { get; set; }
    }

    public class ItemGiveLogData : ItemLogData
    {
        public string? CharacterName { get; set; }
    }

    public class CharacterModifierLogData
    {
        public string? Name { get; set; }
    }
}