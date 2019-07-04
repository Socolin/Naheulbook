using System;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.Servers;
using Naheulbook.Tests.Functional.Code.TestServices;
using Naheulbook.TestUtils;
using Naheulbook.Web.Configurations;
using Naheulbook.Web.Services;
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

        public UserSteps(
            ScenarioContext scenarioContext,
            UserTestService userTestService,
            TestDataUtil testDataUtil
        )
        {
            _scenarioContext = scenarioContext;
            _userTestService = userTestService;
            _testDataUtil = testDataUtil;
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

            var payload = Jose.JWT.Decode<JObject>(jwt, Convert.FromBase64String(NaheulbookApiServer.JwtSigningKey));

            payload.Should().ContainKey("sub");
            payload.Should().ContainKey("exp");
        }
    }
}