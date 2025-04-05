using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.TestUtils;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

namespace Naheulbook.Data.Tests.Integration;

public static class DbUtils
{
    public static TDbContext GetTestDbContext<TDbContext>(bool logSqlQueries = false) where TDbContext : DbContext
    {
        if (typeof(TDbContext) == typeof(NaheulbookDbContext))
            return CreateNaheulbookDbContext(logSqlQueries) as TDbContext;

        return null;
    }

    public static NaheulbookDbContext CreateNaheulbookDbContext(bool logSqlQueries = false)
    {
        var dbContextOptions = GetDbContextOptions<NaheulbookDbContext>(logSqlQueries);

        return new NaheulbookDbContext(dbContextOptions);
    }

    public static DbContextOptions<T> GetDbContextOptions<T>(bool logSqlQueries = false)
        where T : DbContext
    {
        var connectionString = GetConnectionString();
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<T>()
            .EnableSensitiveDataLogging()
            .UseMySQL(connectionString, builder => builder.EnableRetryOnFailure());

        if (logSqlQueries)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: ConsoleTheme.None)
                .CreateLogger();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new SerilogLoggerProvider(logger));
            dbContextOptionsBuilder.UseLoggerFactory(loggerFactory);
        }

        return dbContextOptionsBuilder.Options;
    }

    public static string GetConnectionString()
    {
        return $"Server={Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost"};" +
               $"Port={DockerServicePortFinder.GetPortForServiceInDocker("scripts-naheulbook_dev_env_mysql-1", "3306/tcp")?.ToString() ?? Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306"};" +
               "Database=naheulbook_integration;" +
               "User=naheulbook;" +
               "Password=naheulbook;" +
               "SslMode=None";
    }
}