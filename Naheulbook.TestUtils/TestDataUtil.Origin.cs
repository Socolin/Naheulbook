using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddOrigin(Action<Origin> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateOrigin(), customizer);
        }
    }
}