using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddMonsterType(Action<MonsterTypeEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterType(), customizer);
        }

        public TestDataUtil AddMonsterSubCategory(Action<MonsterSubCategoryEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterSubCategory(GetLast<MonsterTypeEntity>()), customizer);
        }

        public TestDataUtil AddMonsterTrait(Action<MonsterTraitEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterTrait(), customizer);
        }

        public TestDataUtil AddMonsterTemplate(Action<MonsterTemplateEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonsterTemplate(GetLast<MonsterSubCategoryEntity>()), customizer);
        }

        public TestDataUtil AddMonster(Action<MonsterEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateMonster(GetLast<GroupEntity>()), customizer);
        }
    }
}