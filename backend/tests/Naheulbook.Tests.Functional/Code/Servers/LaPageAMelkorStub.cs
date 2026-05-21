using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Naheulbook.Tests.Functional.Code.Stubs.Melkor;

namespace Naheulbook.Tests.Functional.Code.Servers;

public class LaPageAMelkorStub
{
    public IEnumerable<string> ListenUrls => _server.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>().Addresses;
    private IHost _server;

    public void Start()
    {
        _server = new HostBuilder()
            .ConfigureWebHostDefaults(c => c
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://[::1]:0")
                .UseEnvironment(Environments.Development)
                .UseStartup<Startup>()
            ).Build();

        _server.Start();
    }

    public void Stop()
    {
        _server?.StopAsync().GetAwaiter().GetResult();
        _server?.Dispose();
        _server = null;
    }
}