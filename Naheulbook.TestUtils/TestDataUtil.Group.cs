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
    }
}