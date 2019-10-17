using System;
using System.IO;
using System.Linq;
using BoDi;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Tests.Functional.Code.Servers;
using Socolin.TestsUtils.FakeSmtp;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init
{
    [Binding]
    public class ServerInitializer
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ScenarioContext _scenarioContext;

        public ServerInitializer(
            IObjectContainer objectContainer,
            ScenarioContext scenarioContext
        )
        {
            _objectContainer = objectContainer;
            _scenarioContext = scenarioContext;
        }

        private static NaheulbookApiServer _naheulbookApiServer;
        private static LaPageAMelkorStub _laPageAMelkorStub;
        private static FakeSmtpServer _fakeSmtpServer;
        private static string _mapImageOutputDirectory;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _fakeSmtpServer = new FakeSmtpServer();
            var mailConfig = _fakeSmtpServer.Start();
            _mapImageOutputDirectory = Path.Combine(Path.GetTempPath(), "map-" + Guid.NewGuid());
            Directory.CreateDirectory(_mapImageOutputDirectory);

            _laPageAMelkorStub = new LaPageAMelkorStub();
            _laPageAMelkorStub.Start();
            _naheulbookApiServer = new NaheulbookApiServer(
                mailConfig,
                _laPageAMelkorStub.ListenUrls.First(),
                _mapImageOutputDirectory
            );
            _naheulbookApiServer.Start();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _laPageAMelkorStub.Stop();
            _naheulbookApiServer.Stop();
            _fakeSmtpServer.Stop();
            _fakeSmtpServer.Dispose();
            Directory.Delete(_mapImageOutputDirectory, true);
        }

        [BeforeScenario(Order = 0)]
        public void InitializeIoc()
        {
            _objectContainer.RegisterInstanceAs(_fakeSmtpServer, typeof(IMailReceiver));
            _objectContainer.RegisterInstanceAs(_naheulbookApiServer, typeof(NaheulbookApiServer));
            _objectContainer.RegisterInstanceAs(new NaheulbookHttpClient(_naheulbookApiServer.ListenUrls.First()));
            _scenarioContext.SetMapImageOutputDirectory(_mapImageOutputDirectory);
        }
    }
}