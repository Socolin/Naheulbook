using BoDi;
using Naheulbook.Tests.Functional.Code.TestServices;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init;

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
        _objectContainer.RegisterTypeAs<EffectTestService, EffectTestService>();
        _objectContainer.RegisterTypeAs<UserTestService, UserTestService>();
        _objectContainer.RegisterTypeAs<TestDataUtil, TestDataUtil>();
    }
}