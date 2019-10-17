using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Naheulbook.Web.Sentry;
using Sentry;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Naheulbook.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environment))
                throw new Exception("ASPNETCORE_ENVIRONMENT environment variable should be set");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json")
                .AddJsonFile($"appsettings.local.json", true)
                .AddCommandLine(args)
                .Build();

            using (SentrySdk.Init(configuration["Sentry:DSN"]))
            {
                var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .WriteTo.Sentry()
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                    .CreateLogger();

                var server = new WebHostBuilder()
                    .UseLibuv()
                    .ConfigureKestrel((context, options) =>
                    {
                        options.Limits.MaxRequestBodySize = 60000000;
                        if (!string.IsNullOrEmpty(context.Configuration["socket"]))
                            options.ListenUnixSocket(context.Configuration["socket"]);
                    })
                    .UseKestrel()
                    .UseSentry(o => o.BeforeSend = DefaultSentryEventExceptionProcessor.BeforeSend)
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseEnvironment(environment)
                    .UseStartup<Startup>()
                    .UseSerilog(logger)
                    .UseConfiguration(configuration)
                    .Build();

                server.Run();
            }
        }
    }
}