using BoDi;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Tests.Functional.Code.TestServices;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init
{
    [Binding]
    public class IocInitializer
    {
        private readonly IObjectContainer _objectContainer;

        public IocInitializer(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }


        [BeforeScenario(Order = 0)]
        public void InitializeIoc()
        {
            _objectContainer.RegisterTypeAs<UserTestService, UserTestService>();
        }
    }
}