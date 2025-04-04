using System;
using System.Collections.Generic;

namespace Naheulbook.Core.Features.Character.Backup.V1;

public class BackupCharacter : Backup.BackupCharacter
{
    public BackupCharacter()
    {
        Version = 1;
    }

    public class BaseStats
    {
        public int Ad { get; set; }
        public int Int { get; set; }
        public int Cou { get; set; }
        public int Fo { get; set; }
        public int Cha { get; set; }
    }

    public class CharacterModifier
    {
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

        public ICollection<CharacterModifierValue> Values { get; set; } = null!;
    }

    public class CharacterModifierValue
    {
        public string StatName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public short Value { get; set; }
    }

    public BaseStats Stats { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Sex { get; set; } = null!;
    public int? Ev { get; set; }
    public int? Ea { get; set; }
    public short FatePoint { get; set; }
    public string? StatBonusAd { get; set; }
    public string? Notes { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public Guid OriginId { get; set; }
    public List<Guid> JobIds { get; set; } = null!;
    public List<Guid> SkillIds { get; set; } = null!;
    public List<Guid> SpecialitiesIds { get; set; } = null!;
    public List<CharacterModifier> Modifiers { get; set; } = null!;
    public List<BackupCharacterItem> Items { get; set; } = null!;
}