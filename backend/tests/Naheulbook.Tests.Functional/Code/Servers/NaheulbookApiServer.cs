using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Naheulbook.Tests.Functional.Code.Constants;
using Naheulbook.Web;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using Socolin.TestUtils.FakeSmtp;

namespace Naheulbook.Tests.Functional.Code.Servers;

public class NaheulbookApiServer(
    FakeSmtpConfig mailConfig,
    string laPageAMelkorUrl,
    string mapImageOutputDirectory
)
{
    public const string JwtSigningKey = "jUPS+BG/+FxexuNitsuiIHWXOLTZb3yQSxyLpOfTo2/BB8MNUZcNP+13cvAlPP5O";
    public IEnumerable<string> ListenUrls => _server.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>().Addresses;
    private IHost _server;

    public void Start()
    {
        var testConfiguration = new Dictionary<string, string>
        {
            ["Authentication:JwtSigningKey"] = JwtSigningKey,
            ["ConnectionStrings:DefaultConnection"] = DefaultTestConfigurations.NaheulbookTestConnectionString,
            ["Mail:Smtp:Host"] = mailConfig.Host.ToString(),
            ["Mail:Smtp:Port"] = mailConfig.Port.ToString(),
            ["Mail:Smtp:Username"] = mailConfig.Username,
            ["Mail:Smtp:Password"] = mailConfig.Password,
            ["Mail:Smtp:Ssl"] = false.ToString(),
            ["Mail:FromAddress"] = "some-address@some-domain.aa",
            ["LaPageAMelkor:Url"] = laPageAMelkorUrl,
            ["MapImage:OutputDirectory"] = mapImageOutputDirectory,
            ["Authentication:Facebook:AppId"] = "unused",
            ["Authentication:Facebook:AppSecret"] = "unused",
            ["Authentication:MicrosoftGraph:AppId"] = "unused",
            ["Authentication:MicrosoftGraph:AppSecret"] = "unused",
            ["Authentication:Twitter:AppId"] = "unused",
            ["Authentication:Twitter:AppSecret"] = "unused",
            ["Authentication:Twitter:Callback"] = "unused",
            ["Authentication:Google:AppId"] = "unused",
            ["Authentication:Google:AppSecret"] = "unused",
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(testConfiguration)
            .AddEnvironmentVariables()
            .Build();

        var logger = new LoggerConfiguration()
            .WriteTo.Console(theme: ConsoleTheme.None)
            .WriteTo.File(new CompactJsonFormatter(), "logs/naheulbook.api.json")
            .CreateLogger();

        _server = new HostBuilder()
            .ConfigureWebHost(builder =>
            {
                builder
                    .UseUrls("http://[::1]:0")
                    .UseStartup<Startup>()
                    .UseConfiguration(configuration)
                    .UseKestrel();
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseEnvironment(Environments.Development)
            .UseSerilog(logger)
            .Build();

        _server.Start();
    }

    public void Stop()
    {
        _server?.StopAsync().GetAwaiter().GetResult();
        _server?.WaitForShutdownAsync().GetAwaiter().GetResult();
        _server?.Dispose();
        _server = null;
    }
}