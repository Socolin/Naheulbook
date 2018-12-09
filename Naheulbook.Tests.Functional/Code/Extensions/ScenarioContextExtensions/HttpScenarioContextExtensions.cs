using System.Net;
using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.FakeSmtp;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions
{
    public static class HttpScenarioContextExtensions
    {
        private const string LastHttpResponseStatusCodeKey = "LastHttpResponseStatusCode";
        private const string LastHttpResponseContentKey = "LastHttpResponseContent";

        public static void SetLastHttpResponseStatusCode(this ScenarioContext scenarioContext, HttpStatusCode response)
        {
            scenarioContext.Set(response, LastHttpResponseStatusCodeKey);
        }

        public static HttpStatusCode GetLastHttpResponseStatusCode(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<HttpStatusCode>(LastHttpResponseStatusCodeKey);
        }

        public static void SetLastHttpResponseContent(this ScenarioContext scenarioContext, string response)
        {
            scenarioContext.Set(response, LastHttpResponseContentKey);
        }

        public static string GetLastHttpResponseContent(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<string>(LastHttpResponseContentKey);
        }

        public static JToken GetLastJsonHttpResponseContent(this ScenarioContext scenarioContext)
        {
            return JToken.Parse(scenarioContext.Get<string>(LastHttpResponseContentKey));
        }

    }
}