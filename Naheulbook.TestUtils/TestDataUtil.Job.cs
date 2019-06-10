using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddJob(Action<Job> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateJob(), customizer);
        }
    }
}