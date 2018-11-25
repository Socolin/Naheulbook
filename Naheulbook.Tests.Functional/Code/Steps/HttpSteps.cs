using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Tests.Functional.Code.Extensions;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Socolin.TestsUtils.Comparer.Json;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Steps
{
    [Binding]
    public class HttpSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly NaheulbookHttpClient _naheulbookHttpClient;

        public HttpSteps(ScenarioContext scenarioContext, NaheulbookHttpClient naheulbookHttpClient)
        {
            _scenarioContext = scenarioContext;
            _naheulbookHttpClient = naheulbookHttpClient;
        }

        [When(@"performing a GET to the url ""(.*)""")]
        public async Task WhenPerformingAGetToTheUrl(string url)
        {
            var response = await _naheulbookHttpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            _scenarioContext.SetLastHttpResponseStatusCode(response.StatusCode);
            _scenarioContext.SetLastHttpResponseContent(content);
        }

        [When(@"performing a POST to the url ""(.*)"" with the following ""(.+)"" content")]
        public async Task WhenPerformingAPostToTheUrl(string url, string mimeType, string contentData)
        {
            var requestContent = new StringContent(contentData.ExecuteReplacement(_scenarioContext), Encoding.UTF8, mimeType);
            var response = await _naheulbookHttpClient.PostAsync(url, requestContent);
            var content = await response.Content.ReadAsStringAsync();
            _scenarioContext.SetLastHttpResponseStatusCode(response.StatusCode);
            _scenarioContext.SetLastHttpResponseContent(content);
        }

        [Then(@"the response status code be (.*)")]
        public void ThenTheResponseStatusCodeBe(int expectedStatusCode)
        {
            var lastStatusCode = _scenarioContext.GetLastHttpResponseStatusCode();
            if ((int) lastStatusCode != expectedStatusCode)
                Assert.Fail($"Expected {nameof(lastStatusCode)} to be {expectedStatusCode} but found {lastStatusCode} with content '{_scenarioContext.GetLastHttpResponseContent()}'");
        }

        [Then(@"the response should contains the following json")]
        public void TheResponseShouldContainsTheFollowingJson(int expectedStatusCode, string expectedJson)
        {
            var lastStatusCode = _scenarioContext.GetLastHttpResponseStatusCode();
            var content = _scenarioContext.GetLastHttpResponseContent();
            lastStatusCode.Should().Be(expectedStatusCode);

            var jsonComparer = JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expectedJson, content);
            if (errors.Count == 0)
                return;

            Assert.Fail(string.Join("\n", errors.Select(error => $"{error.Path}: {error.Message}")));
        }

        [Then(@"the response should contains a json array containing the following element identified by (.+)")]
        public void TheResponseShouldContainsAJsonArrayContainingTheFollowingElementIdentifiedBy(string identityField, string expectedJson)
        {
            var content = _scenarioContext.GetLastHttpResponseContent();

            var jsonComparer = JsonComparer.GetDefault();
            var expectedObject = JObject.Parse(expectedJson);
            var identityValue = ((JValue) expectedObject.Property(identityField).Value).Value<long>();
            var array = JArray.Parse(content);

            foreach (var element in array)
            {
                if (element.Type != JTokenType.Object || !(element is JObject actualObject))
                    continue;

                if (identityValue != ((JValue) actualObject.Property(identityField).Value).Value<long>())
                    continue;

                var errors = jsonComparer.Compare(expectedObject, element).ToList();
                if (errors.Count == 0)
                    return;

                Assert.Fail(JsonComparerOutputFormatter.GetReadableMessage(expectedObject, actualObject, errors));
            }

            Assert.Fail($"Failed to find expected element using `{identityField}`.\nExpected identifier value: `{identityValue}` but found values: {string.Join(",", array.Select(s => s[identityField]))}\nResponse:\n{content}");
        }

    }
}