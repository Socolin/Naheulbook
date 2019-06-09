using Naheulbook.Tests.Functional.Code.Extensions;
using Naheulbook.TestUtils;
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
            private readonly TestDataUtil _testDataUtil;

            public StringReplacementTransform(ScenarioContext scenarioContext, TestDataUtil testDataUtil)
            {
                _scenarioContext = scenarioContext;
                _testDataUtil = testDataUtil;
            }

            [StepArgumentTransformation]
            public JToken ReplaceJToken(string input)
            {
                return JToken.Parse(input.ExecuteReplacement(_scenarioContext, _testDataUtil));
            }
        }
    }
}