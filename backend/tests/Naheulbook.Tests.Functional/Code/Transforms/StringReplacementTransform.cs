using Naheulbook.Tests.Functional.Code.Extensions;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Transforms;

[Binding]
public class StringReplacementTransform(ScenarioContext scenarioContext, TestDataUtil testDataUtil)
{
    [StepArgumentTransformation]
    public string ReplacementTransform(string input)
    {
        return input.ExecuteReplacement(scenarioContext, testDataUtil);
    }
}