using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddStat(Action<Stat> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateStat(), customizer);
        }
    }
}