using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Naheulbook.DatabaseMigrator;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration;

[SetUpFixture]
public class DbInitializer
{
    [OneTimeSetUp]
    public void PrepareTests()
    {
        try
        {
            using var dbContext = new DbContext(DbUtils.GetDbContextOptions<DbContext>());
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            dbContext.Database.ExecuteSqlRaw("ALTER DATABASE `naheulbook_integration` CHARACTER SET UTF8 COLLATE utf8_general_ci");

            var services = new ServiceCollection();
            services.AddDatabaseMigrator(DbUtils.GetConnectionString());
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}