using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var json = scenarioContext.Get<string>(LastHttpResponseContentKey);
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("Last request returned No Content");
            }
            try
            {
                return JToken.Parse(json);
            }
            catch (JsonReaderException ex)
            {
                throw new Exception($"An error occured while parsing json\n\t{ex.Message}\nJson:\n{json}");
            }
        }
    }
}