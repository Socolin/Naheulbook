using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class CharacterModifierEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }

        public bool Permanent { get; set; }
        public bool Reusable { get; set; }

        public string DurationType { get; set; } = null!;
        public string? Duration { get; set; }
        public int? TimeDuration { get; set; }
        public int? CombatCount { get; set; }
        public int? LapCount { get; set; }
        public string? LapCountDecrement { get; set; }

        public int? CurrentCombatCount { get; set; }
        public int? CurrentTimeDuration { get; set; }
        public int? CurrentLapCount { get; set; }

        // TODO: Rework Characterhistory so this dont have to be nullable
        public int? CharacterId { get; set; }
        public CharacterEntity? Character { get; set; }

        public ICollection<CharacterModifierValueEntity> Values { get; set; } = null!;
    }
}