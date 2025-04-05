using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.DatabaseMigrator;
using Naheulbook.Tests.Functional.Code.Constants;
using Reqnroll;
using Reqnroll.BoDi;

namespace Naheulbook.Tests.Functional.Code.Init;

[Binding]
public class DatabaseInitializer(IObjectContainer objectContainer)
{
    [BeforeTestRun]
    public static void InitializeDatabase()
    {
        using (InitializersProfiler.Profile(nameof(InitializeDatabase)))
        {
            var dbContextOptions = new DbContextOptionsBuilder<DbContext>()
                .UseMySQL(DefaultTestConfigurations.NaheulbookTestConnectionString, builder => builder.EnableRetryOnFailure())
                .Options;

            using var dbContext = new DbContext(dbContextOptions);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        using (InitializersProfiler.Profile(nameof(InitializeDatabase) + "_Migrate"))
        {
            var services = new ServiceCollection();
            services.AddDatabaseMigrator(DefaultTestConfigurations.NaheulbookTestConnectionString);
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }


    [BeforeScenario(Order = 0)]
    public void InitializeIoc()
    {
        var dbContextOptions = new DbContextOptionsBuilder<NaheulbookDbContext>()
            .UseMySQL(DefaultTestConfigurations.NaheulbookTestConnectionString, builder => builder.EnableRetryOnFailure())
            .Options;
        objectContainer.RegisterInstanceAs(dbContextOptions);
    }
}