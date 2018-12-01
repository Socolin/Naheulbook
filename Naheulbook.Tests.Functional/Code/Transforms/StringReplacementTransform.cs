using Naheulbook.Tests.Functional.Code.Extensions;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Transforms
{
    [Binding]
    public class StringReplacementTransform
    {
        private readonly ScenarioContext _scenarioContext;

        public StringReplacementTransform(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [StepArgumentTransformation]
        public string ReplacementTransform(string input)
        {
            return input.ExecuteReplacement(_scenarioContext);
        }
    }
}