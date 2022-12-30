using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Naheulbook.Web.Sentry;
using Sentry;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Naheulbook.Web;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (string.IsNullOrEmpty(environment))
            throw new Exception("ASPNETCORE_ENVIRONMENT or DOTNET_ENVIRONMENT environment variable should be set");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json")
            .AddJsonFile("appsettings.local.json", true)
            .AddCommandLine(args)
            .Build();

        using (SentrySdk.Init(configuration["Sentry:DSN"]))
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Sentry()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            var server = new HostBuilder()
                .ConfigureWebHost(builder =>
                {
                    builder.ConfigureKestrel((context, options) =>
                        {
                            options.Limits.MaxRequestBodySize = 60000000;
                            var unixSocketPath = context.Configuration.GetValue<string>("socket");
                            if (File.Exists(unixSocketPath))
                                File.Delete(unixSocketPath);
                            if (!string.IsNullOrEmpty(unixSocketPath))
                                options.ListenUnixSocket(unixSocketPath);
                        })
                        .UseKestrel()
                        .UseSentry(o => o.BeforeSend = DefaultSentryEventExceptionProcessor.BeforeSend)
                        .UseConfiguration(configuration)
                        .UseStartup<Startup>();
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment(environment)
                .UseSerilog(logger)
                .Build();

            await server.StartAsync();
            logger.Information("Server started.");

            var unixSocketPath = configuration.GetValue<string>("socket");
            var unixSocketPermission = configuration.GetValue<string>("socketPermission");

            if (OperatingSystem.IsLinux() && !string.IsNullOrEmpty(unixSocketPath) && !string.IsNullOrEmpty(unixSocketPermission))
            {
                logger.Information("Changing socket permission {Socket} to {Permission}.", unixSocketPath, unixSocketPermission);
                var fileMode = Convert.ToInt32(unixSocketPermission, 8);
                File.SetUnixFileMode(unixSocketPath, (UnixFileMode)fileMode);
            }

            await server.WaitForShutdownAsync();

            if (OperatingSystem.IsLinux() && !string.IsNullOrEmpty(unixSocketPath))
            {
                logger.Information("Server started.");
                File.Delete(unixSocketPath);
            }
        }
    }
}