using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddMonsterType(Action<MonsterType> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterType(), customizer);
        }

        public TestDataUtil AddMonsterCategory(Action<MonsterCategory> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterCategory(GetLast<MonsterType>()), customizer);
        }

        public TestDataUtil AddMonsterTrait(Action<MonsterTrait> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterTrait(), customizer);
        }
    }
}