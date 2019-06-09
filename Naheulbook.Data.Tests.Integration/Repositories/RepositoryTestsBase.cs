using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.TestUtils;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class RepositoryTestsBase<TDbContext> where TDbContext : DbContext
    {
        protected TDbContext RepositoryDbContext;
        protected  TestDataUtil TestDataUtil;
        private TDbContext _addEntitiesContext;
        private List<object> _allEntities;

        [SetUp]
        public void BaseSetUp()
        {
            _allEntities = new List<object>();
            _addEntitiesContext = DbUtils.GetTestDbContext<TDbContext>();
            RepositoryDbContext = DbUtils.GetTestDbContext<TDbContext>();
            TestDataUtil = new TestDataUtil(DbUtils.GetDbContextOptions(), new DefaultEntityCreator());

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
            _allEntities.Clear();
            _addEntitiesContext?.Dispose();
        }

        protected async Task<List<TEntity>> AddInDbAsync<TEntity>(params TEntity[] entities) where TEntity : class
        {
            return await AddInDbAsync(entities.ToList());
        }

        protected async Task<List<TEntity>> AddInDbAsync<TEntity>(List<TEntity> entities) where TEntity : class
        {
            _addEntitiesContext.AddRange(entities);
            await _addEntitiesContext.SaveChangesAsync();

            _allEntities.AddRange(entities);
            return entities;
        }
    }
}