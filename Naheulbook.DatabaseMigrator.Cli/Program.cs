using System;
using System.IO;
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
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.local.json", true)
                .AddCommandLine(args)
                .Build();
            var serviceProvider = CreateServices(config["ConnectionStrings:DefaultConnection"]);

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider, config["operation"]);
            }
        }

        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .Configure<FluentMigratorLoggerOptions>(
                    opt =>
                    {
                        opt.ShowElapsedTime = true;
                        opt.ShowSql = true;
                    })
                .ConfigureRunner(rb => rb
                    .AddMySql5()
                    .WithGlobalConnectionString(connectionString + ";Allow User Variables=True")
                    .ScanIn(typeof(Mig0001Init).Assembly).For.Migrations().For.EmbeddedResources())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider, string operation)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            if (operation == "rollback")
            {
                runner.Rollback(1);
            }
            else
            {
                runner.MigrateUp();
            }
        }
    }
}