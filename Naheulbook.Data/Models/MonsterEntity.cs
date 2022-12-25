using System;
using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MonsterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Data { get; set; }
        public DateTimeOffset? Dead { get; set; }

        public string? Modifiers { get; set; }

        public int GroupId { get; set; }
        public GroupEntity Group { get; set; } = null!;

        public int? LootId { get; set; }
        public LootEntity? Loot { get; set; }

        public int? TargetedCharacterId { get; set; }
        public CharacterEntity? TargetedCharacter { get; set; }

        public int? TargetedMonsterId { get; set; }
        public MonsterEntity? TargetedMonster { get; set; }

        public ICollection<ItemEntity> Items { get; set; } = null!;
    }
}