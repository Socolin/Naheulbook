using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Naheulbook.Tests.Functional.Code.Constants;
using Naheulbook.Web;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using Socolin.TestsUtils.FakeSmtp;

namespace Naheulbook.Tests.Functional.Code.Servers
{
    public class NaheulbookApiServer
    {
        private readonly FakeSmtpConfig _mailConfig;
        public static readonly Uri Url = new Uri("http://localhost:7894");

        private IWebHost _server;

        public NaheulbookApiServer(FakeSmtpConfig mailConfig)
        {
            _mailConfig = mailConfig;
        }

        public void Start()
        {
            var testConfiguration = new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = DefaultTestConfigurations.NaheulbookTestConnectionString,
                ["Mail:Smtp:Host"] = _mailConfig.Host.ToString(),
                ["Mail:Smtp:Port"] = _mailConfig.Port.ToString(),
                ["Mail:Smtp:Username"] = _mailConfig.Username,
                ["Mail:Smtp:Password"] = _mailConfig.Password,
                ["Mail:Smtp:Ssl"] = false.ToString(),
                ["Mail:FromAddress"] = "some-address@some-domain.aa",
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(testConfiguration)
                .AddEnvironmentVariables()
                .Build();

            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: ConsoleTheme.None)
                .WriteTo.File(new CompactJsonFormatter(), "logs/naheulbook.api.json")
                .CreateLogger();

            _server = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls(Url.ToString())
                .UseEnvironment(EnvironmentName.Development)
                .UseStartup<Startup>()
                .UseSerilog(logger)
                .UseConfiguration(configuration)
                .Build();

            _server.Start();
        }

        public void Stop()
        {
            _server?.StopAsync().GetAwaiter().GetResult();
            _server?.Dispose();
        }
    }
}