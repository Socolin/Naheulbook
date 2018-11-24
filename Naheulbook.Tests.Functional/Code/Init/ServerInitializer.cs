using Naheulbook.Tests.Functional.Code.Servers;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init
{
    [Binding]
    public class ServerInitializer
    {
        private static NaheulbookApiServer _naheulbookApiServer;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _naheulbookApiServer = new NaheulbookApiServer();
            _naheulbookApiServer.Start();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _naheulbookApiServer.Stop();
        }
    }
}