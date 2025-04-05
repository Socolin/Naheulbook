using System.Diagnostics;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Naheulbook.DatabaseMigrator.Migrations;

namespace Naheulbook.DatabaseMigrator.Cli;

internal class Program
{
    private static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.local.json", true)
            .AddCommandLine(args)
            .Build();
        var connectionString = config["ConnectionStrings:DefaultConnection"];
        if (connectionString == null)
            throw new Exception("Missing configuration ConnectionStrings:DefaultConnection");
        var operation = config["operation"];
        if (operation == null)
            throw new Exception("Missing --operation");

        var serviceProvider = CreateServices(connectionString, operation);

        using var scope = serviceProvider.CreateScope();
        UpdateDatabase(scope.ServiceProvider, operation);
    }

    private static IServiceProvider CreateServices(string connectionString, string operation)
    {
        var asm = typeof(Mig0039Init).Assembly;
        foreach (var n in asm.GetManifestResourceNames())
            Debug.WriteLine(n);

        return new ServiceCollection()
            .AddFluentMigratorCore()
            .Configure<FluentMigratorLoggerOptions>(
                opt =>
                {
                    opt.ShowElapsedTime = true;
                    opt.ShowSql = true;
                })
            .Configure<RunnerOptions>(
                opt =>
                {
                    if (operation == "init")
                        opt.Profile = "InitialData";
                })
            .ConfigureRunner(rb => rb
                .AddMySql5()
                .WithGlobalConnectionString(connectionString + ";Allow User Variables=True")
                .ScanIn(typeof(Mig0039Init).Assembly).For.Migrations().For.EmbeddedResources())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }

    private static void UpdateDatabase(IServiceProvider serviceProvider, string operation)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        switch (operation)
        {
            case "rollback":
                runner.Rollback(1);
                break;
            case "init":
                runner.MigrateUp();
                break;
            default:
                runner.MigrateUp();
                break;
        }
    }
}