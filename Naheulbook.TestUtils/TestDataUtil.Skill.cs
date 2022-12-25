using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddSkill(Action<SkillEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateSkill(), customizer);
        }
    }
}