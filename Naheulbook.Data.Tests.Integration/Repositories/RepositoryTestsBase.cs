using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.TestUtils;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class RepositoryTestsBase<TDbContext> where TDbContext : DbContext
    {
        protected TDbContext RepositoryDbContext;
        protected TestDataUtil TestDataUtil;

        [SetUp]
        public void BaseSetUp()
        {
            RepositoryDbContext = DbUtils.GetTestDbContext<TDbContext>(true);
            TestDataUtil = new TestDataUtil(DbUtils.GetDbContextOptions(), new DefaultEntityCreator());
            TestDataUtil.Cleanup();
        }

        [TearDown]
        public async Task BaseTearDown()
        {
            RepositoryDbContext?.Dispose();
            using (var dbContext = DbUtils.GetTestDbContext<TDbContext>())
            {
                dbContext.RemoveRange(_allEntities);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}