using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public EffectType CreateEffectType(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new EffectType
            {
                Name = $"some-effect-type-name-{suffix}"
            };
        }

        public EffectSubCategory CreateEffectSubCategory(EffectType effectType, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new EffectSubCategory
            {
                Name = $"some-effect-category-name-{suffix}",
                Note = $"some-effect-name-{suffix}",
                DiceCount = 1,
                DiceSize = 20,
                Type = effectType
            };
        }

        public Effect CreateEffect(EffectSubCategory subCategory, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Effect
            {
                SubCategory = subCategory,
                Description = $"some-description-{suffix}",
                Dice = 4,
                Name = $"some-effect-name-{suffix}",
                DurationType = "combat",
                CombatCount = 2,
                Modifiers = new List<EffectModifier>
                {
                    new EffectModifier
                    {
                        StatName = "CHA",
                        Value = 1,
                        Type = "ADD"
                    },
                    new EffectModifier
                    {
                        StatName = "FO",
                        Value = 4,
                        Type = "ADD"
                    },
                    new EffectModifier
                    {
                        StatName = "INT",
                        Value = -2,
                        Type = "ADD"
                    },
                }
            };
        }
    }
}