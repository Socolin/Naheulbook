using System.Net.Http.Headers;
using System.Text;
using ImageMagick;
using ImageMagick.Drawing;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Socolin.TestUtils.JsonComparer;
using Socolin.TestUtils.JsonComparer.Color;
using Socolin.TestUtils.JsonComparer.NUnitExtensions;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.Steps;

[Binding]
public class HttpSteps(
    ScenarioContext scenarioContext,
    NaheulbookHttpClient naheulbookHttpClient,
    IJsonComparer jsonComparer,
    JsonComparerColorOptions jsonComparerColorOptions
)
{
    [When("""^performing a (GET|DELETE) to the url "(.*)"( with the current jwt| with ".+" as access token|)$""")]
    public async Task WhenPerformingAGetToTheUrlWithToken(string method, string url, string useCurrentJwt)
    {
        var httpRequestMessage = new HttpRequestMessage(new HttpMethod(method), url);
        if (!string.IsNullOrEmpty(useCurrentJwt))
        {
            if (useCurrentJwt.StartsWith(" with the current jwt"))
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("JWT", scenarioContext.GetJwt());
            else if (useCurrentJwt.EndsWith("as access token"))
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "userAccessToken:" + useCurrentJwt.Split('"')[1]);
        }

        var response = await naheulbookHttpClient.SendAsync(httpRequestMessage);
        var content = await response.Content.ReadAsStringAsync();
        scenarioContext.SetLastHttpResponseStatusCode((int)response.StatusCode);
        scenarioContext.SetLastHttpResponseContent(content);
    }

    [When(@"^performing a (GET|DELETE) to the url ""(.*)"" with the current session$")]
    public async Task WhenPerformingAGetToTheUrlWithTheCurrentSession(string method, string url)
    {
        using var httpClient = new HttpClient(new HttpClientHandler {CookieContainer = scenarioContext.GetHttpCookiesContainer()});
        httpClient.BaseAddress = naheulbookHttpClient.BaseAddress;

        foreach (var (name, value) in naheulbookHttpClient.DefaultRequestHeaders)
            httpClient.DefaultRequestHeaders.Add(name, value);

        var httpRequestMessage = new HttpRequestMessage(new HttpMethod(method), url);
        var response = await httpClient.SendAsync(httpRequestMessage);
        var content = await response.Content.ReadAsStringAsync();

        scenarioContext.SetLastHttpResponseStatusCode((int)response.StatusCode);
        scenarioContext.SetLastHttpResponseContent(content);
    }

    [When(@"performing a POST to the url {string} with the following json content")]
    public async Task WhenPerformingAPostToTheUrlWithContent(string url, string contentData)
    {
        var requestContent = new StringContent(contentData, Encoding.UTF8, "application/json");
        var response = await naheulbookHttpClient.PostAsync(url, requestContent);
        var content = await response.Content.ReadAsStringAsync();
        scenarioContext.SetLastHttpResponseStatusCode((int)response.StatusCode);
        scenarioContext.SetLastHttpResponseContent(content);
    }

    [When(@"performing a POST to the url {string} with the following json content and the current session")]
    public async Task WhenPerformingAPostToTheUrlWithContentAndTheCurrentSession(string url, string contentData)
    {
        var requestContent = new StringContent(contentData, Encoding.UTF8, "application/json");
        using var httpClient = new HttpClient(new HttpClientHandler {CookieContainer = scenarioContext.GetHttpCookiesContainer()});
        httpClient.BaseAddress = naheulbookHttpClient.BaseAddress;

        foreach (var (name, value) in naheulbookHttpClient.DefaultRequestHeaders)
            httpClient.DefaultRequestHeaders.Add(name, value);

        var response = await httpClient.PostAsync(url, requestContent);
        var content = await response.Content.ReadAsStringAsync();

        scenarioContext.SetLastHttpResponseStatusCode((int)response.StatusCode);
        scenarioContext.SetLastHttpResponseContent(content);
    }

    [When("performing a {word} to the url {string} with the following json content and the current jwt")]
    public async Task WhenPerformingAPostToTheUrlWithContentAndJwt(string method, string url, string contentData)
    {
        var httpRequestMessage = new HttpRequestMessage(new HttpMethod(method), url)
        {
            Content = new StringContent(contentData, Encoding.UTF8, "application/json"),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("JWT", scenarioContext.GetJwt()),
            },
        };
        var response = await naheulbookHttpClient.SendAsync(httpRequestMessage);
        var content = await response.Content.ReadAsStringAsync();
        scenarioContext.SetLastHttpResponseStatusCode((int)response.StatusCode);
        scenarioContext.SetLastHttpResponseContent(content);
    }

    [When("performing a multipart POST to the url {string} with the following json content as {string} and an image as {string} and the current jwt")]
    public async Task WhenPerformingAMultipartPostToTheUrlWithContentAndJwt(string url, string contentPartName, string imagePartName, string contentData)
    {
        using var image = new MagickImage(new MagickColor("#ff00ff"), 1024, 512);
        using var memoryStream = new MemoryStream();
        new Drawables().Line(0, 0, 1024, 512).Draw(image);
        await image.WriteAsync(memoryStream, MagickFormat.Png);

        var imageContent = new ByteArrayContent(memoryStream.GetBuffer())
        {
            Headers =
            {
                ContentType = new MediaTypeHeaderValue("image/png"),
            },
        };

        var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new StringContent(contentData, Encoding.UTF8, "application/json"), contentPartName);
        multipartContent.Add(imageContent, imagePartName, "some-image.png");

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = multipartContent,
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("JWT", scenarioContext.GetJwt()),
            },
        };
        var response = await naheulbookHttpClient.SendAsync(httpRequestMessage);
        var content = await response.Content.ReadAsStringAsync();
        scenarioContext.SetLastHttpResponseStatusCode((int)response.StatusCode);
        scenarioContext.SetLastHttpResponseContent(content);
    }

    [Then(@"^the response status code is (\d+)$")]
    public void ThenTheResponseStatusCodeBe(int expectedStatusCode)
    {
        var lastStatusCode = scenarioContext.GetLastHttpResponseStatusCode();
        if (lastStatusCode != expectedStatusCode)
        {
            if (string.IsNullOrEmpty(scenarioContext.GetLastHttpResponseContent()))
                Assert.Fail($"Expected {nameof(lastStatusCode)} to be {expectedStatusCode} but found {lastStatusCode} with no content");
            else
                Assert.Fail($"Expected {nameof(lastStatusCode)} to be {expectedStatusCode} but found {lastStatusCode} with content:\n{scenarioContext.GetLastHttpResponseContent()}\n");
        }
    }

    [Then(@"^the response should contains the following json$")]
    public void TheResponseShouldContainsTheFollowingJson(JToken expectedJson)
    {
        var jsonContent = scenarioContext.GetLastJsonHttpResponseContent();

        Assert.That(jsonContent, IsJson.EquivalentTo(expectedJson).WithComparer(jsonComparer).WithColorOptions(jsonComparerColorOptions));
    }
}
