using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Naheulbook.Data.DbContexts;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

namespace Naheulbook.Data.Tests.Integration
{
    public static class DbUtils
    {
        public static TDbContext GetTestDbContext<TDbContext>() where TDbContext : DbContext
        {
            if (typeof(TDbContext) == typeof(NaheulbookDbContext))
                return GetNaheulbookDbContext() as TDbContext;

            return null;
        }

        public static NaheulbookDbContext GetNaheulbookDbContext()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: ConsoleTheme.None)
                .CreateLogger();

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new SerilogLoggerProvider(logger));

            var dbContextOptions = new DbContextOptionsBuilder<NaheulbookDbContext>()
                .ConfigureWarnings(w => w.Throw(RelationalEventId.QueryClientEvaluationWarning))
                .EnableSensitiveDataLogging()
                .UseMySql("Server=127.0.0.1;Database=naheulbook_integration;User=naheulbook;Password=naheulbook;SslMode=None")
                .UseLoggerFactory(loggerFactory)
                .Options;

            return new NaheulbookDbContext(dbContextOptions);
        }
    }
}