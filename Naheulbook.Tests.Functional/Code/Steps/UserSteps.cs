using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
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
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Steps
{
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

        [Given("a user identified by a password")]
        public async Task GivenAUserIdentifiedByAPassword()
        {
            var (username, password, userId) = await _userTestService.CreateUserAsync();

            _scenarioContext.SetUsername(username);
            _scenarioContext.SetPassword(password);
            _scenarioContext.SetUserId(userId);
        }

        [Given("a user")]
        public void GivenAUser()
        {
            _testDataUtil.AddUser();
        }

        [Given("a JWT for a user")]
        public void GivenAJwtForAUser()
        {
            var hasUtil = new PasswordHashingService();
            _testDataUtil.AddUser(u => u.HashedPassword = hasUtil.HashPassword("some-password"));

            _scenarioContext.SetUsername(_testDataUtil.GetLast<User>().Username);
            _scenarioContext.SetPassword("some-password");
            _scenarioContext.SetUserId(_testDataUtil.GetLast<User>().Id);

            var jwtService = new JwtService(new AuthenticationConfiguration {JwtSigningKey = NaheulbookApiServer.JwtSigningKey, JwtExpirationDelayInMinutes = 10}, new TimeService());
            var jwt = jwtService.GenerateJwtToken(_testDataUtil.GetLast<User>().Id);

            _scenarioContext.SetJwt(jwt);
        }

        [Given("a JWT for an admin user")]
        public void GivenAnAdminUser()
        {
            var hasUtil = new PasswordHashingService();
            _testDataUtil.AddUser(u =>
            {
                u.HashedPassword = hasUtil.HashPassword("some-password");
                u.Admin = true;
            });

            _scenarioContext.SetUsername(_testDataUtil.GetLast<User>().Username);
            _scenarioContext.SetPassword("some-password");
            _scenarioContext.SetUserId(_testDataUtil.GetLast<User>().Id);

            var jwtService = new JwtService(new AuthenticationConfiguration {JwtSigningKey = NaheulbookApiServer.JwtSigningKey, JwtExpirationDelayInMinutes = 10}, new TimeService());
            var jwt = jwtService.GenerateJwtToken(_testDataUtil.GetLast<User>().Id);

            _scenarioContext.SetJwt(jwt);
        }

        [Then("the response content contains a valid JWT")]
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

        [Given("that the owner of the character allow to appear in searches")]
        public void GivenThatTheOwnerOfTheCharacterAllowToAppearInSearches()
        {
            _testDataUtil.Get<Character>().Owner.ShowInSearchUntil = DateTime.UtcNow.AddDays(1);
            _testDataUtil.SaveChanges();
        }

        [Given("the user is authenticated with a session")]
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
                    Password = _scenarioContext.GetPassword()
                }), Encoding.UTF8, "application/json")
            };
            var response = await httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
        }
    }
}