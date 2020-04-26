using System;

namespace Naheulbook.Data.Models
{
    public class CharacterHistoryEntry : IHistoryEntry
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public string? Data { get; set; }
        public DateTime Date { get; set; }
        public bool Gm { get; set; }

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;
    }
}