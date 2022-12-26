using System.Collections.Generic;
using Naheulbook.Data.Extensions;

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
        private CharacterEntity? _character;
        public CharacterEntity? Character { get => _character.ThrowIfNotLoadedAndNotNull(CharacterId); set => _character = value; }

        private ICollection<CharacterModifierValueEntity>? _values;
        public ICollection<CharacterModifierValueEntity> Values { get => _values.ThrowIfNotLoaded(); set => _values = value; }
    }
}