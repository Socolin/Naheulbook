using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class EffectType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<EffectCategory> Categories { get; set; } = null!;
    }

    public class EffectCategory
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public short DiceCount { get; set; }

        public short DiceSize { get; set; }

        public string? Note { get; set; }

        public int TypeId { get; set; }

        public EffectType Type { get; set; } = null!;

        public ICollection<Effect> Effects { get; set; } = null!;
    }

    public class Effect
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string DurationType { get; set; } = null!;

        public string? Duration { get; set; }

        public int? CombatCount { get; set; }

        public int? LapCount { get; set; }

        public int? TimeDuration { get; set; }

        public short? Dice { get; set; }

        public int CategoryId { get; set; }

        public ICollection<EffectModifier> Modifiers { get; set; } = null!;

        public EffectCategory Category { get; set; } = null!;
    }


    public class EffectModifier
    {
        public int EffectId { get; set; }

        public string StatName { get; set; } = null!;

        public short Value { get; set; }

        public string Type { get; set; } = null!;

        public Effect Effect { get; set; } = null!;

        public Stat Stat { get; set; } = null!;
    }
}