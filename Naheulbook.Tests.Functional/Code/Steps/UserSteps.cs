using System.Text.RegularExpressions;
using Naheulbook.Tests.Functional.Code.Extensions;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Steps
{
    public class UserSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public UserSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }
    }
}