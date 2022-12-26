using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddEffectType(Action<EffectTypeEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEffectType(), customizer);
        }

        public TestDataUtil AddEffectSubCategory(Action<EffectSubCategoryEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEffectSubCategory(GetLast<EffectTypeEntity>()), customizer);
        }

        public TestDataUtil AddEffect(Action<EffectEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEffect(GetLast<EffectSubCategoryEntity>()), customizer);
        }
    }
}