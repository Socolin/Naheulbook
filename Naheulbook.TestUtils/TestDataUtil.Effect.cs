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

        public TestDataUtil AddEffectCategory(Action<EffectCategory> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEffectCategory(GetLast<EffectType>()), customizer);
        }

        public TestDataUtil AddEffect(Action<Effect> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEffect(GetLast<EffectCategory>()), customizer);
        }
    }
}