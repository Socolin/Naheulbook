#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.TestUtils;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class RepositoryTestsBase<TDbContext> where TDbContext : DbContext
    {
        protected TDbContext RepositoryDbContext => _repositoryDbContext ?? throw new NullReferenceException($"{nameof(_repositoryDbContext)} was not initialized");
        private TDbContext? _repositoryDbContext;
        protected TestDataUtil TestDataUtil = null!;

        [SetUp]
        public void BaseSetUp()
        {
            _repositoryDbContext = DbUtils.GetTestDbContext<TDbContext>(true);
            TestDataUtil = new TestDataUtil(DbUtils.GetDbContextOptions(), new DefaultEntityCreator());
            TestDataUtil.Cleanup();
        }

        [TearDown]
        public async Task BaseTearDown()
        {
            if (_repositoryDbContext != null)
                await _repositoryDbContext.DisposeAsync();

            await using var dbContext = DbUtils.GetTestDbContext<TDbContext>();
            await dbContext.SaveChangesAsync();
        }
    }
}