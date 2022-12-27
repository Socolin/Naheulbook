using System;
using System.Collections.Generic;
using System.Linq;
using BoDi;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Naheulbook.Data.DbContexts;
using Naheulbook.DatabaseMigrator.Migrations;
using Naheulbook.Tests.Functional.Code.Constants;
using Naheulbook.Tests.Functional.Code.Tools;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init;

[Binding]
public class DatabaseInitializer
{
    private readonly IObjectContainer _objectContainer;

    public DatabaseInitializer(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    [BeforeTestRun]
    public static void InitializeDatabase()
    {
        try
        {
            using var _ = InitializersProfiler.Profile(nameof(InitializeDatabase));
            var serviceProvider = new ServiceCollection()
                .Configure<FluentMigratorLoggerOptions>(
                    opt =>
                    {
                        opt.ShowElapsedTime = true;
                        opt.ShowSql = true;
                    })
                .AddLogging(lb => lb
                    .AddFluentMigratorConsole()
                    .Services.AddSingleton<ILoggerProvider>(services => new FluentMigratorFileLoggerProvider("/tmp/fluentmigrator.log", services.GetService<IOptions<FluentMigratorLoggerOptions>>()))
                )
                .AddFluentMigratorCore()
                .ConfigureRunner(
                    builder => builder
                        .AddMySql5()
                        .WithGlobalConnectionString(DefaultTestConfigurations.NaheulbookTestConnectionString)
                        .ScanIn(typeof(Mig0001Init).Assembly).For.EmbeddedResources()
                        .WithMigrationsIn(typeof(Mig0001Init).Assembly))
                .BuildServiceProvider();


            var dbContextOptions = new DbContextOptionsBuilder<DbContext>()
                .UseMySql(DefaultTestConfigurations.NaheulbookTestConnectionString, ServerVersion.AutoDetect(DefaultTestConfigurations.NaheulbookTestConnectionString), builder => builder.EnableRetryOnFailure())
                .Options;

            DropAllTables(dbContextOptions);

            using (var scope = serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private static void DropAllTables(DbContextOptions<DbContext> dbContextOptions)
    {
        var tablesNames = new List<string>();

        using (var dbContext = new DbContext(dbContextOptions))
        using (var command = dbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "SELECT `table_name` FROM information_schema.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = @DbName";
            var dbNameParameter = command.CreateParameter();
            dbNameParameter.ParameterName = "@DbName";
            dbNameParameter.Value = DefaultTestConfigurations.NaheulbookDbName;
            command.Parameters.Add(dbNameParameter);

            dbContext.Database.GetDbConnection().Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tablesNames.Add(reader.GetString(0));
            }
        }

        using (var dbContext = new DbContext(dbContextOptions))
        {
            dbContext.Database.ExecuteSqlRaw(
                "SET foreign_key_checks = 0;\n" +
                string.Join('\n', tablesNames.Select(tableName => $"DROP TABLE `{tableName}`;")) +
                "SET foreign_key_checks = 1\n"
            );
        }
    }

    [BeforeScenario(Order = 0)]
    public void InitializeIoc()
    {
        var dbContextOptions = new DbContextOptionsBuilder<NaheulbookDbContext>()
            .UseMySql(DefaultTestConfigurations.NaheulbookTestConnectionString, ServerVersion.AutoDetect(DefaultTestConfigurations.NaheulbookTestConnectionString), builder => builder.EnableRetryOnFailure())
            .Options;
        _objectContainer.RegisterInstanceAs(dbContextOptions);
    }
}