using BoDi;
using Naheulbook.Tests.Functional.Code.HttpClients;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init
{
    [Binding]
    public class HttpClientInitializer
    {
        private readonly IObjectContainer _objectContainer;

        public HttpClientInitializer(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void InitializeWebDriver()
        {
            _objectContainer.RegisterInstanceAs(new NaheulbookHttpClient());
        }
    }
}