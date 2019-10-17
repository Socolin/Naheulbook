using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
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
        private readonly string _laPageAMelkorUrl;
        private readonly string _mapImageOutputDirectory;
        public const string JwtSigningKey = "jUPS+BG/+FxexuNitsuiIHWXOLTZb3yQSxyLpOfTo2/BB8MNUZcNP+13cvAlPP5O";
        public IEnumerable<string> ListenUrls => _server.ServerFeatures.Get<IServerAddressesFeature>().Addresses;
        private IWebHost _server;

        public NaheulbookApiServer(
            FakeSmtpConfig mailConfig,
            string laPageAMelkorUrl,
            string mapImageOutputDirectory
        )
        {
            _mailConfig = mailConfig;
            _laPageAMelkorUrl = laPageAMelkorUrl;
            _mapImageOutputDirectory = mapImageOutputDirectory;
        }

        public void Start()
        {
            var testConfiguration = new Dictionary<string, string>
            {
                ["Authentication:JwtSigningKey"] = JwtSigningKey,
                ["ConnectionStrings:DefaultConnection"] = DefaultTestConfigurations.NaheulbookTestConnectionString,
                ["Mail:Smtp:Host"] = _mailConfig.Host.ToString(),
                ["Mail:Smtp:Port"] = _mailConfig.Port.ToString(),
                ["Mail:Smtp:Username"] = _mailConfig.Username,
                ["Mail:Smtp:Password"] = _mailConfig.Password,
                ["Mail:Smtp:Ssl"] = false.ToString(),
                ["Mail:FromAddress"] = "some-address@some-domain.aa",
                ["LaPageAMelkor:Url"] = _laPageAMelkorUrl,
                ["MapImage:OutputDirectory"] = _mapImageOutputDirectory,
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
                .UseUrls("http://[::1]:0")
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