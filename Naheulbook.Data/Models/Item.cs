namespace Naheulbook.Data.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Data { get; set; } = null!;
        public string? Modifiers { get; set; }
        public int? ContainerId { get; set; }
        public Item? Container { get; set; }

        public int ItemTemplateId { get; set; }
        public ItemTemplate ItemTemplate { get; set; } = null!;

        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        public int? LootId { get; set; }
        public Loot? Loot { get; set; }

        public int? MonsterId { get; set; }
        public Monster? Monster { get; set; }

        public string? LifetimeType { get; set; }
    }
}