using Naheulbook.Tests.Functional.Code.TestServices;
using Naheulbook.TestUtils;
using Reqnroll;
using Reqnroll.BoDi;

namespace Naheulbook.Tests.Functional.Code.Init;

[Binding]
public class IocInitializer(IObjectContainer objectContainer)
{
    [BeforeScenario(Order = 0)]
    public void InitializeIoc()
    {
        objectContainer.RegisterTypeAs<EffectTestService, EffectTestService>();
        objectContainer.RegisterTypeAs<UserTestService, UserTestService>();
        objectContainer.RegisterTypeAs<TestDataUtil, TestDataUtil>();
    }
}