using System.Net;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions
{
    public static class ScenarioContextExtensions
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
    }
}