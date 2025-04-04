using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Naheulbook.DatabaseMigrator.Migrations;
using Naheulbook.DatabaseMigrator.Tools;

namespace Naheulbook.DatabaseMigrator;

public static class ServiceCollectionExtensions
{
    public static void AddDatabaseMigrator(this IServiceCollection services, string connectionString)
    {
        services
            .Configure<FluentMigratorLoggerOptions>(
                opt =>
                {
                    opt.ShowElapsedTime = true;
                    opt.ShowSql = true;
                }
            )
            .AddLogging(lb => lb
                .AddFluentMigratorConsole()
                .Services.AddSingleton<ILoggerProvider>(sp => new FluentMigratorFileLoggerProvider("/tmp/fluentmigrator.log", sp.GetRequiredService<IOptions<FluentMigratorLoggerOptions>>()))
            )
            .AddFluentMigratorCore()
            .ConfigureRunner(
                builder => builder
                    .AddMySql5()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(Mig0039Init).Assembly).For.EmbeddedResources()
                    .WithMigrationsIn(typeof(Mig0039Init).Assembly)
            )
            .BuildServiceProvider();
    }
}