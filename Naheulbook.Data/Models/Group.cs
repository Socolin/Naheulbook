using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }

        public int? CombatLootId { get; set; }
        public Loot CombatLoot { get; set; }

        public int MasterId { get; set; }
        public User Master { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }

        public ICollection<Loot> Loots { get; set; }
        public ICollection<Monster> Monsters { get; set; }
        public ICollection<Character> Characters { get; set; }

        public ICollection<GroupInvite> Invites { get; set; }
    }
}