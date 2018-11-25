using BoDi;
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
        private static FakeSmtpServer _fakeSmtpServer;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _fakeSmtpServer = new FakeSmtpServer();
            var mailConfig = _fakeSmtpServer.Start();
            _naheulbookApiServer = new NaheulbookApiServer(mailConfig);
            _naheulbookApiServer.Start();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _naheulbookApiServer.Stop();
            _fakeSmtpServer.Stop();
            _fakeSmtpServer.Dispose();
        }

        [BeforeScenario]
        public void InitializeWebDriver()
        {
            _objectContainer.RegisterInstanceAs(_fakeSmtpServer, typeof(IMailReceiver));
        }
    }
}