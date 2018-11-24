using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration
{
    [SetUpFixture]
    public class DbInitializer
    {
        [OneTimeSetUp]
        public void PrepareTests()
        {
            var dbContext = DbUtils.GetNaheulbookDbContext();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        }
    }
}