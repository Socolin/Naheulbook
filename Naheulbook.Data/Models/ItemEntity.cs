using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class ItemEntity
    {
        public int Id { get; set; }

        public string Data { get; set; } = null!;
        public string? Modifiers { get; set; }

        public int? ContainerId { get; set; }
        private ItemEntity? _container;
        public ItemEntity? Container { get => _container.ThrowIfNotLoadedAndNotNull(ContainerId); set => _container = value; }

        public Guid ItemTemplateId { get; set; }
        private ItemTemplateEntity? _itemTemplate;
        public ItemTemplateEntity ItemTemplate { get => _itemTemplate.ThrowIfNotLoaded(); set => _itemTemplate = value; }

        public int? CharacterId { get; set; }
        private CharacterEntity? _character;
        public CharacterEntity? Character { get => _character.ThrowIfNotLoadedAndNotNull(CharacterId); set => _character = value; }

        public int? LootId { get; set; }
        private LootEntity? _loot;
        public LootEntity? Loot { get => _loot.ThrowIfNotLoadedAndNotNull(LootId); set => _loot = value; }

        public int? MonsterId { get; set; }
        private MonsterEntity? _monster;
        public MonsterEntity? Monster { get => _monster.ThrowIfNotLoadedAndNotNull(MonsterId); set => _monster = value; }

        public string? LifetimeType { get; set; }
    }
}