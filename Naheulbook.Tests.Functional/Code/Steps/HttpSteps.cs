using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Socolin.TestUtils.JsonComparer;
using Socolin.TestUtils.JsonComparer.NUnitExtensions;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Steps
{
    [Binding]
    public class HttpSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly NaheulbookHttpClient _naheulbookHttpClient;
        private readonly IJsonComparer _jsonComparer;

        public HttpSteps(ScenarioContext scenarioContext, NaheulbookHttpClient naheulbookHttpClient, IJsonComparer jsonComparer)
        {
            _scenarioContext = scenarioContext;
            _naheulbookHttpClient = naheulbookHttpClient;
            _jsonComparer = jsonComparer;
        }

        [When(@"performing a (GET|DELETE) to the url ""(.*)""( with the current jwt)?")]
        public async Task WhenPerformingAGetToTheUrl(string method, string url, string useCurrentJwt)
        {
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(method), url);
            if (!string.IsNullOrEmpty(useCurrentJwt))
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("JWT", _scenarioContext.GetJwt());
            var response = await _naheulbookHttpClient.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadAsStringAsync();
            _scenarioContext.SetLastHttpResponseStatusCode((int) response.StatusCode);
            _scenarioContext.SetLastHttpResponseContent(content);
        }

        [When(@"performing a POST to the url ""(.*)"" with the following json content")]
        public async Task WhenPerformingAPostToTheUrlWithContent(string url, string contentData)
        {
            var requestContent = new StringContent(contentData, Encoding.UTF8, "application/json");
            var response = await _naheulbookHttpClient.PostAsync(url, requestContent);
            var content = await response.Content.ReadAsStringAsync();
            _scenarioContext.SetLastHttpResponseStatusCode((int) response.StatusCode);
            _scenarioContext.SetLastHttpResponseContent(content);
        }

        [When(@"performing a ([A-Z]+) to the url ""(.*)"" with the following json content and the current jwt")]
        public async Task WhenPerformingAPostToTheUrlWithContentAndJwt(string method, string url, string contentData)
        {
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(method), url)
            {
                Content = new StringContent(contentData, Encoding.UTF8, "application/json"),
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("JWT", _scenarioContext.GetJwt())
                }
            };
            var response = await _naheulbookHttpClient.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadAsStringAsync();
            _scenarioContext.SetLastHttpResponseStatusCode((int) response.StatusCode);
            _scenarioContext.SetLastHttpResponseContent(content);
        }

        [When(@"performing a multipart POST to the url ""(.*)"" with the following json content as ""(.+)"" and an image as ""(.+)"" and the current jwt")]
        public async Task WhenPerformingAMultipartPostToTheUrlWithContentAndJwt(string url, string contentPartName, string imagePartName, string contentData)
        {
            using var image = new MagickImage(new MagickColor("#ff00ff"), 1024, 512);
            using var memoryStream = new MemoryStream();
            new Drawables().Line(0, 0, 1024, 512).Draw(image);
            image.Write(memoryStream, MagickFormat.Png);

            var imageContent = new ByteArrayContent(memoryStream.GetBuffer())
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("image/png")
                }
            };

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(contentData, Encoding.UTF8, "application/json"), contentPartName);
            multipartContent.Add(imageContent, imagePartName, "some-image.png");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = multipartContent,
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("JWT", _scenarioContext.GetJwt())
                }
            };
            var response = await _naheulbookHttpClient.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadAsStringAsync();
            _scenarioContext.SetLastHttpResponseStatusCode((int) response.StatusCode);
            _scenarioContext.SetLastHttpResponseContent(content);
        }

        [Then(@"the response status code is (.*)")]
        public void ThenTheResponseStatusCodeBe(int expectedStatusCode)
        {
            var lastStatusCode = _scenarioContext.GetLastHttpResponseStatusCode();
            if (lastStatusCode != expectedStatusCode)
            {
                if (string.IsNullOrEmpty(_scenarioContext.GetLastHttpResponseContent()))
                    Assert.Fail($"Expected {nameof(lastStatusCode)} to be {expectedStatusCode} but found {lastStatusCode} with no content");
                else
                    Assert.Fail($"Expected {nameof(lastStatusCode)} to be {expectedStatusCode} but found {lastStatusCode} with content:\n{_scenarioContext.GetLastHttpResponseContent()}\n");
            }
        }

        [Then(@"the response should contains the following json")]
        public void TheResponseShouldContainsTheFollowingJson(JToken expectedJson)
        {
            var jsonContent = _scenarioContext.GetLastJsonHttpResponseContent();

            Assert.That(jsonContent, IsJson.EquivalentTo(expectedJson).WithComparer(_jsonComparer).WithColoredOutput());
        }

        [Then(@"the response should contains a json array containing the following element identified by (.+)")]
        public void TheResponseShouldContainsAJsonArrayContainingTheFollowingElementIdentifiedBy(string identityField, string expectedJson)
        {
            var content = _scenarioContext.GetLastHttpResponseContent();

            JObject expectedObject;
            try
            {
                expectedObject = JObject.Parse(expectedJson);
            }
            catch (JsonReaderException ex)
            {
                throw new Exception($"Invalid expected JSON: {ex.Message} Line:\n '{expectedJson.Split('\n')[ex.LineNumber - 1]}'", ex);
            }

            var identityValue = (JValue) expectedObject.Property(identityField).Value;
            var array = JArray.Parse(content);

            foreach (var element in array)
            {
                if (element.Type != JTokenType.Object || !(element is JObject actualObject))
                    continue;

                if (!identityValue.Equals((JValue) actualObject.Property(identityField).Value))
                    continue;

                Assert.That(element, IsJson.EquivalentTo(expectedObject).WithComparer(_jsonComparer).WithColoredOutput());
                return;
            }

            Assert.Fail($"Failed to find expected element using `{identityField}`.\nExpected identifier value: `{identityValue}` but found values: {string.Join(",", array.Select(s => s[identityField]))}\nResponse:\n{content}");
        }
    }
}