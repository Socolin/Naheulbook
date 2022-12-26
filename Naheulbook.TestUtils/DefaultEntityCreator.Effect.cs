using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public EffectTypeEntity CreateEffectType(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new EffectTypeEntity
            {
                Name = $"some-effect-type-name-{suffix}"
            };
        }

        public EffectSubCategoryEntity CreateEffectSubCategory(EffectTypeEntity effectType, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new EffectSubCategoryEntity
            {
                Name = $"some-effect-sub-category-name-{suffix}",
                Note = $"some-effect-name-{suffix}",
                DiceCount = 1,
                DiceSize = 20,
                Type = effectType
            };
        }

        public EffectEntity CreateEffect(EffectSubCategoryEntity subCategory, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new EffectEntity
            {
                SubCategory = subCategory,
                Description = $"some-description-{suffix}",
                Dice = 4,
                Name = $"some-effect-name-{suffix}",
                DurationType = "combat",
                CombatCount = 2,
                Modifiers = new List<EffectModifierEntity>
                {
                    new EffectModifierEntity
                    {
                        StatName = "CHA",
                        Value = 1,
                        Type = "ADD"
                    },
                    new EffectModifierEntity
                    {
                        StatName = "FO",
                        Value = 4,
                        Type = "ADD"
                    },
                    new EffectModifierEntity
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