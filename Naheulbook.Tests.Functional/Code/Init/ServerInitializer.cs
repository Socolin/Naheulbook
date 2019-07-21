using System.Linq;
using BoDi;
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

        public ServerInitializer(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        private static NaheulbookApiServer _naheulbookApiServer;
        private static LaPageAMelkorStub _laPageAMelkorStub;
        private static FakeSmtpServer _fakeSmtpServer;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _fakeSmtpServer = new FakeSmtpServer();
            var mailConfig = _fakeSmtpServer.Start();
            _laPageAMelkorStub = new LaPageAMelkorStub();
            _laPageAMelkorStub.Start();
            _naheulbookApiServer = new NaheulbookApiServer(mailConfig, _laPageAMelkorStub.ListenUrls.First());
            _naheulbookApiServer.Start();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _laPageAMelkorStub.Stop();
            _naheulbookApiServer.Stop();
            _fakeSmtpServer.Stop();
            _fakeSmtpServer.Dispose();
        }

        [BeforeScenario(Order = 0)]
        public void InitializeIoc()
        {
            _objectContainer.RegisterInstanceAs(_fakeSmtpServer, typeof(IMailReceiver));
            _objectContainer.RegisterInstanceAs(_naheulbookApiServer, typeof(NaheulbookApiServer));
            _objectContainer.RegisterInstanceAs(new NaheulbookHttpClient(_naheulbookApiServer.ListenUrls.First()));
        }
    }
}