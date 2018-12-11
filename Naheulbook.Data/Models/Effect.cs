using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class EffectType
    {
        public EffectType()
        {
            Categories = new HashSet<EffectCategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<EffectCategory> Categories { get; set; }
    }

    public class EffectCategory
    {
        public EffectCategory()
        {
            Effects = new HashSet<Effect>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public short DiceCount { get; set; }

        public short DiceSize { get; set; }

        public string Note { get; set; }

        public int TypeId { get; set; }

        public EffectType Type { get; set; }

        public ICollection<Effect> Effects { get; set; }
    }

    public class Effect
    {
        public Effect()
        {
            Modifiers = new HashSet<EffectModifier>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DurationType { get; set; }

        public string Duration { get; set; }

        public int? CombatCount { get; set; }

        public int? LapCount { get; set; }

        public int? TimeDuration { get; set; }

        public short? Dice { get; set; }

        public int CategoryId { get; set; }

        public ICollection<EffectModifier> Modifiers { get; set; }

        public EffectCategory Category { get; set; }
    }


    public class EffectModifier
    {
        public int EffectId { get; set; }

        public string StatName { get; set; }

        public short Value { get; set; }

        public string Type { get; set; }

        public virtual Effect Effect { get; set; }

        public virtual Stat Stat { get; set; }
    }
}