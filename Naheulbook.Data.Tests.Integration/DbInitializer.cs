using System;
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
            try
            {
                var dbContext = DbUtils.CreateNaheulbookDbContext();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}