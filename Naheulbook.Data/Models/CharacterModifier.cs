using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class CharacterModifier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public bool Permanent { get; set; }
        public bool Reusable { get; set; }

        public string DurationType { get; set; }
        public string Duration { get; set; }
        public int? TimeDuration { get; set; }
        public int? CombatCount { get; set; }
        public int? LapCount { get; set; }
        public string LapCountDecrement { get; set; }

        public int? CurrentCombatCount { get; set; }
        public int? CurrentTimeDuration { get; set; }
        public int? CurrentLapCount { get; set; }

        public int CharacterId { get; set; }
        public Character Character { get; set; }

        public ICollection<CharacterModifierValue> Values { get; set; }
    }
}