using BoDi;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Data.DbContexts;
using Naheulbook.DatabaseMigrator.Migrations;
using Naheulbook.Tests.Functional.Code.Constants;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Tests.Functional.Code.Servers;
using Socolin.TestsUtils.FakeSmtp;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init
{
    [Binding]
    public class DatabaseInitializer
    {
        private readonly IObjectContainer _objectContainer;

        public DatabaseInitializer(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(
                    builder => builder
                        .AddMySql5()
                        .WithGlobalConnectionString(DefaultTestConfigurations.NaheulbookTestConnectionString)
                        .ScanIn(typeof(Mig0001Init).Assembly).For.EmbeddedResources()
                        .WithMigrationsIn(typeof(Mig0001Init).Assembly))
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateDown(0);
                runner.MigrateUp();
            }
        }

        [BeforeScenario(Order = 0)]
        public void InitializeIoc()
        {
            var dbContextOptions = new DbContextOptionsBuilder<NaheulbookDbContext>()
                .UseMySql(DefaultTestConfigurations.NaheulbookTestConnectionString)
                .Options;
            _objectContainer.RegisterInstanceAs(dbContextOptions);
        }
    }
}