using System;

namespace Naheulbook.Data.Models
{
    public class Loot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsVisibleForPlayer { get; set; }
        public DateTime? Created { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}