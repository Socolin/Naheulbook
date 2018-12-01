using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Tests.Functional.Code.Extensions;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Tests.Functional.Code.Servers;
using Naheulbook.Tests.Functional.Code.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.FakeSmtp;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Steps
{
    [Binding]
    public class UserSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IMailReceiver _mailReceiver;
        private readonly NaheulbookHttpClient _naheulbookHttpClient;

        public UserSteps(ScenarioContext scenarioContext, IMailReceiver mailReceiver, NaheulbookHttpClient naheulbookHttpClient)
        {
            _scenarioContext = scenarioContext;
            _mailReceiver = mailReceiver;
            _naheulbookHttpClient = naheulbookHttpClient;
        }

        [Given("a user identified by a password")]
        public async Task GivenAUserIdentifiedByAPassword()
        {
            var username = $"user.{RngUtils.GetRandomHexString(16)}@test.ca";
            var password = RngUtils.GetRandomHexString(32);

            _scenarioContext.SetUsername(username);
            _scenarioContext.SetPassword(password);

            var userRequestJson = JsonConvert.SerializeObject(new {username, password});
            await _naheulbookHttpClient.PostAsync("/api/v2/users/", new StringContent(userRequestJson, Encoding.UTF8, "application/json"));

            var activationCode = MailSteps.ParseActivationCodeFromActivationMail(_mailReceiver.Mails.LastOrDefault(m => m.To.Contains(username)));
            _scenarioContext.SetActivationCode(activationCode);
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