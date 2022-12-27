using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddEffectType(Action<EffectTypeEntity> customizer = null)
        {
            return AddEffectType(out _);
        }

        public TestDataUtil AddEffectType(out EffectTypeEntity effectType, Action<EffectTypeEntity> customizer = null)
        {
            effectType = _defaultEntityCreator.CreateEffectType();
            return SaveEntity(effectType, customizer);
        }

        public TestDataUtil AddEffectSubCategory(Action<EffectSubCategoryEntity> customizer = null)
        {
            return AddEffectSubCategory(out _, customizer);
        }

        public TestDataUtil AddEffectSubCategory(out EffectSubCategoryEntity effectSubCategory, Action<EffectSubCategoryEntity> customizer = null)
        {
            var effectType = GetLast<EffectTypeEntity>();
            effectSubCategory = _defaultEntityCreator.CreateEffectSubCategory(effectType);
            return SaveEntity(effectSubCategory, customizer);
        }

        public TestDataUtil AddEffect(Action<EffectEntity> customizer = null)
        {
            return AddEffect(out _, customizer);
        }

        public TestDataUtil AddEffect(out EffectEntity effect, Action<EffectEntity> customizer = null)
        {
            var effectSubCategory = GetLast<EffectSubCategoryEntity>();
            effect = _defaultEntityCreator.CreateEffect(effectSubCategory);
            return SaveEntity(effect, customizer);
        }

        public TestDataUtil AddEffectModifier(Action<EffectModifierEntity> customizer = null)
        {
            return AddEffectModifier(out _, customizer);
        }

        public TestDataUtil AddEffectModifier(out EffectModifierEntity effectModifier, Action<EffectModifierEntity> customizer = null)
        {
            var effect = GetLast<EffectEntity>();
            var stat = GetLast<StatEntity>();
            effectModifier = new EffectModifierEntity
            {
                EffectId = effect.Id,
                StatName = stat.Name,
                Value = 12,
                Type = "ADD",
            };
            return SaveEntity(effectModifier, customizer);
        }
    }
}