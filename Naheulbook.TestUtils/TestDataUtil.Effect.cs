using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddEffectType(Action<EffectType> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEffectType(), customizer);
        }

        public TestDataUtil AddEffectSubCategory(Action<EffectSubCategory> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEffectSubCategory(GetLast<EffectType>()), customizer);
        }

        public TestDataUtil AddEffect(Action<EffectEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEffect(GetLast<EffectSubCategory>()), customizer);
        }
    }
}