using System;
using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Loot
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsVisibleForPlayer { get; set; }
        public DateTime? Created { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public ICollection<Monster> Monsters { get; set; } = null!;
        public ICollection<Item> Items { get; set; } = null!;
    }
}