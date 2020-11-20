using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions
{
    public static class HttpScenarioContextExtensions
    {
        private const string LastHttpResponseStatusCodeKey = "LastHttpResponseStatusCode";
        private const string LastHttpResponseContentKey = "LastHttpResponseContent";

        public static void SetLastHttpResponseStatusCode(this ScenarioContext scenarioContext, int statusCode)
        {
            scenarioContext.Set(statusCode, LastHttpResponseStatusCodeKey);
        }

        public static int GetLastHttpResponseStatusCode(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<int>(LastHttpResponseStatusCodeKey);
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
                return JsonConvert.DeserializeObject<JToken>(json, new JsonSerializerSettings()
                {
                    DateParseHandling = DateParseHandling.None
                });
            }
            catch (JsonReaderException ex)
            {
                throw new Exception($"An error occured while parsing json\n\t{ex.Message}\nJson:\n{json}");
            }
        }
    }
}