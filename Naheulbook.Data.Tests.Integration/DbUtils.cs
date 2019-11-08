using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Naheulbook.Data.DbContexts;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

namespace Naheulbook.Data.Tests.Integration
{
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
            var dbContextOptions = GetDbContextOptions(logSqlQueries);

            return new NaheulbookDbContext(dbContextOptions);
        }

        public static DbContextOptions<NaheulbookDbContext> GetDbContextOptions(bool logSqlQueries = false)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<NaheulbookDbContext>()
                .EnableSensitiveDataLogging()
                .UseMySql("Server=127.0.0.1;Database=naheulbook_integration;User=naheulbook;Password=naheulbook;SslMode=None");

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
    }
}