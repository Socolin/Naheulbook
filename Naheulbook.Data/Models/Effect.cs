using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Data.Models
{
    public class EffectType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<EffectSubCategory> SubCategories { get; set; } = null!;
    }

    public class EffectSubCategory
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

        public int SubCategoryId { get; set; }
        public EffectSubCategory SubCategory { get; set; } = null!;

        public ICollection<EffectModifier> Modifiers { get; set; } = null!;
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