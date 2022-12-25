using System;
using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class LootEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsVisibleForPlayer { get; set; }
        public DateTime? Created { get; set; }

        public int GroupId { get; set; }
        public GroupEntity Group { get; set; } = null!;

        public ICollection<MonsterEntity> Monsters { get; set; } = null!;
        public ICollection<ItemEntity> Items { get; set; } = null!;
    }
}