using System;

namespace Naheulbook.Data.Models
{
    public class ItemEntity
    {
        public int Id { get; set; }

        public string Data { get; set; } = null!;
        public string? Modifiers { get; set; }
        public int? ContainerId { get; set; }
        public ItemEntity? Container { get; set; }

        public Guid ItemTemplateId { get; set; }
        public ItemTemplateEntity ItemTemplate { get; set; } = null!;

        public int? CharacterId { get; set; }
        public CharacterEntity? Character { get; set; }

        public int? LootId { get; set; }
        public LootEntity? Loot { get; set; }

        public int? MonsterId { get; set; }
        public MonsterEntity? Monster { get; set; }

        public string? LifetimeType { get; set; }
    }
}