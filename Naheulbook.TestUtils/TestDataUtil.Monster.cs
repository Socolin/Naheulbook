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

        public TestDataUtil AddMonsterSubCategory(Action<MonsterSubCategory> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterSubCategory(GetLast<MonsterType>()), customizer);
        }

        public TestDataUtil AddMonsterTrait(Action<MonsterTrait> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterTrait(), customizer);
        }

        public TestDataUtil AddMonsterTemplate(Action<MonsterTemplate> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterTemplate(GetLast<MonsterSubCategory>()), customizer);
        }

        public TestDataUtil AddMonster(Action<Monster> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonster(GetLast<Group>()), customizer);
        }
    }
}