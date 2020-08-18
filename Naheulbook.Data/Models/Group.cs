using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Data { get; set; }
        public string? Config { get; set; }

        public int? CombatLootId { get; set; }
        public Loot? CombatLoot { get; set; }

        public int MasterId { get; set; }
        public User Master { get; set; } = null!;

        public ICollection<Loot> Loots { get; set; } = null!;
        public ICollection<Monster> Monsters { get; set; } = null!;
        public ICollection<Character> Characters { get; set; } = null!;
        public ICollection<GroupInvite> Invites { get; set; } = null!;
        public ICollection<Event> Events { get; set; } = null!;
        public ICollection<GroupHistoryEntry> HistoryEntries { get; set; } = null!;
        public ICollection<Npc> Npcs { get; set; } = null!;

        public void AddHistoryEntry(GroupHistoryEntry entry)
        {
            if (HistoryEntries == null)
                HistoryEntries = new List<GroupHistoryEntry>();
            HistoryEntries.Add(entry);
        }
    }
}