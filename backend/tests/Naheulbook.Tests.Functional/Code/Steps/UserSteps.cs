using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Naheulbook.Core.Features.Users;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Tests.Functional.Code.Servers;
using Naheulbook.Tests.Functional.Code.TestServices;
using Naheulbook.TestUtils;
using Naheulbook.Web.Configurations;
using Naheulbook.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.Steps;

[Binding]
public class UserSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly UserTestService _userTestService;
    private readonly TestDataUtil _testDataUtil;
    private readonly NaheulbookApiServer _naheulbookApiServer;

    public UserSteps(
        ScenarioContext scenarioContext,
        UserTestService userTestService,
        TestDataUtil testDataUtil,
        NaheulbookHttpClient naheulbookHttpClient,
        NaheulbookApiServer naheulbookApiServer
    )
    {
        _scenarioContext = scenarioContext;
        _userTestService = userTestService;
        _testDataUtil = testDataUtil;
        _naheulbookApiServer = naheulbookApiServer;
    }

    [Given("^a user identified by a password$")]
    public async Task GivenAUserIdentifiedByAPassword()
    {
        var (username, password, userId) = await _userTestService.CreateUserAsync();

        _scenarioContext.SetUsername(username);
        _scenarioContext.SetPassword(password);
        _scenarioContext.SetUserId(userId);

        await using var dbContext = _testDataUtil.CreateDbContext();
        var user = await dbContext.Users.SingleAsync(x => x.Username == username);
        _testDataUtil.AddStaticObject(user);
    }

    [Given("^a user$")]
    public void GivenAUser()
    {
        _testDataUtil.AddUser();
    }

    [Given("^a JWT for a user$")]
    public void GivenAJwtForAUser()
    {
        var hasUtil = new PasswordHashingService();
        _testDataUtil.AddUser(u => u.HashedPassword = hasUtil.HashPassword("some-password"));

        _scenarioContext.SetUsername(_testDataUtil.GetLast<UserEntity>().Username);
        _scenarioContext.SetPassword("some-password");
        _scenarioContext.SetUserId(_testDataUtil.GetLast<UserEntity>().Id);

        var authenticationOptions = new AuthenticationOptions {JwtSigningKey = NaheulbookApiServer.JwtSigningKey, JwtExpirationDelayInMinutes = 10};
        var jwtService = new JwtService(Options.Create(authenticationOptions), new TimeService());
        var jwt = jwtService.GenerateJwtToken(_testDataUtil.GetLast<UserEntity>().Id);

        _scenarioContext.SetJwt(jwt);
    }

    [Given("^a JWT for an admin user$")]
    public void GivenAnAdminUser()
    {
        var hasUtil = new PasswordHashingService();
        _testDataUtil.AddUser(u =>
        {
            u.HashedPassword = hasUtil.HashPassword("some-password");
            u.Admin = true;
        });

        _scenarioContext.SetUsername(_testDataUtil.GetLast<UserEntity>().Username);
        _scenarioContext.SetPassword("some-password");
        _scenarioContext.SetUserId(_testDataUtil.GetLast<UserEntity>().Id);

        var authenticationOptions = new AuthenticationOptions {JwtSigningKey = NaheulbookApiServer.JwtSigningKey, JwtExpirationDelayInMinutes = 10};
        var jwtService = new JwtService(Options.Create(authenticationOptions), new TimeService());
        var jwt = jwtService.GenerateJwtToken(_testDataUtil.GetLast<UserEntity>().Id);

        _scenarioContext.SetJwt(jwt);
    }

    [Then("^the response content contains a valid JWT$")]
    public void ThenTheResponseContentContainsAValidJwt()
    {
        var responseContent = _scenarioContext.GetLastHttpResponseContent();
        var response = JObject.Parse(responseContent);

        response.Should().ContainKey("token");
        var jwt = response.Value<string>("token");
        _scenarioContext.SetJwt(jwt);

        var payload = Jose.JWT.Decode<Dictionary<string, dynamic>>(jwt, Convert.FromBase64String(NaheulbookApiServer.JwtSigningKey));

        payload.Should().ContainKey("sub");
        payload.Should().ContainKey("exp");
    }

    [Given("^that the owner of the character allow to appear in searches$")]
    public void GivenThatTheOwnerOfTheCharacterAllowToAppearInSearches()
    {
        _testDataUtil.Get<CharacterEntity>().Owner.ShowInSearchUntil = DateTime.UtcNow.AddDays(1);
        _testDataUtil.SaveChanges();
    }

    [Given("^the user is authenticated with a session$")]
    public async Task GivenTheUserIsAuthenticatedWithASession()
    {
        var cookies = new CookieContainer();
        _scenarioContext.SetHttpCookiesContainer(cookies);
        var handler = new HttpClientHandler {CookieContainer = cookies};
        using var httpClient = new HttpClient(handler);

        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(new Uri(_naheulbookApiServer.ListenUrls.First()), $"/api/v2/users/{_scenarioContext.GetUsername()}/jwt"),
            Content = new StringContent(JsonConvert.SerializeObject(new GenerateJwtRequest
            {
                Password = _scenarioContext.GetPassword(),
            }), Encoding.UTF8, "application/json"),
        };
        var response = await httpClient.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
    }
}