using Naheulbook.Tests.Functional.Code.Extensions;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Transforms;

[Binding]
public class StringReplacementTransform
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TestDataUtil _testDataUtil;

    public StringReplacementTransform(ScenarioContext scenarioContext, TestDataUtil testDataUtil)
    {
        _scenarioContext = scenarioContext;
        _testDataUtil = testDataUtil;
    }

    [StepArgumentTransformation]
    public string ReplacementTransform(string input)
    {
        return input.ExecuteReplacement(_scenarioContext, _testDataUtil);
    }
}