using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddGroup(int masterId, Action<Group> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateGroup(masterId, GetLast<Location>()), customizer);
        }

        public TestDataUtil AddGroup(Action<Group> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateGroup(GetLast<User>().Id, GetLast<Location>()), customizer);
        }

        public TestDataUtil AddLoot(Action<Loot> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateLoot(GetLast<Group>()), customizer);
        }
    }
}