using Naheulbook.Tests.Functional.Code.Extensions;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Transforms
{
    public class JTokenTransform
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
            public JToken ReplaceJToken(string input)
            {
                return JToken.Parse(input.ExecuteReplacement(_scenarioContext));
            }
        }
    }
}