using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddNpc(Action<Npc> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateNpc(GetLast<Group>()), customizer);
        }
    }
}