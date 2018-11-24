using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Naheulbook.DatabaseMigrator.Migrations;
using Naheulbook.Tests.Functional.Code.Constants;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init
{
    [Binding]
    public class DatabaseInitializer
    {
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
    }
}