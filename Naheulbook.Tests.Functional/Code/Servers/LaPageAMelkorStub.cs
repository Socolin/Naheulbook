using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Naheulbook.Tests.Functional.Code.Stubs.Melkor;

namespace Naheulbook.Tests.Functional.Code.Servers
{
    public class LaPageAMelkorStub
    {
        public IEnumerable<string> ListenUrls => _server.ServerFeatures.Get<IServerAddressesFeature>().Addresses;
        private IWebHost _server;

        public void Start()
        {
            _server = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://[::1]:0")
                .UseEnvironment(EnvironmentName.Development)
                .UseStartup<Startup>()
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