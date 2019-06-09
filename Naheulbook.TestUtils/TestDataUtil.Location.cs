using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddLocation(Action<Location> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateLocation(), customizer);
        }
    }
}