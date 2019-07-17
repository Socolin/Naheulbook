using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Naheulbook.DatabaseMigrator.Migrations;

namespace Naheulbook.DatabaseMigrator.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.local.json", true)
                .AddCommandLine(args)
                .Build();
            var serviceProvider = CreateServices(config["ConnectionStrings:DefaultConnection"]);

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddMySql5()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(Mig0001Init).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }
    }
}