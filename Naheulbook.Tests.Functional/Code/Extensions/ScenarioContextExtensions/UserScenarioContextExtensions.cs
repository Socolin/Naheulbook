using Socolin.TestsUtils.FakeSmtp;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions
{
    public static class UserScenarioContextExtensions
    {
        private const string LastReceivedMailKey = "LastReceivedMail";
        private const string UsernameKey = "Username";
        private const string PasswordKey = "Password";
        private const string ActivationCodeKey = "ActivationCode";
        private const string JwtKey = "Jwt";

        public static void SetLastReceivedMail(this ScenarioContext scenarioContext, FakeSmtpMail mail)
        {
            scenarioContext.Set(mail, LastReceivedMailKey);
        }

        public static FakeSmtpMail GetLastReceivedMail(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<FakeSmtpMail>(LastReceivedMailKey);
        }

        public static void SetUsername(this ScenarioContext scenarioContext, string username)
        {
            scenarioContext.Set(username, UsernameKey);
        }

        public static string GetUsername(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<string>(UsernameKey);
        }

        public static void SetPassword(this ScenarioContext scenarioContext, string username)
        {
            scenarioContext.Set(username, PasswordKey);
        }

        public static string GetPassword(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<string>(PasswordKey);
        }

        public static void SetActivationCode(this ScenarioContext scenarioContext, string activationCode)
        {
            scenarioContext.Set(activationCode, ActivationCodeKey);
        }

        public static string GetActivationCode(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<string>(ActivationCodeKey);
        }

        public static void SetJwt(this ScenarioContext scenarioContext, string jwt)
        {
            scenarioContext.Set(jwt, JwtKey);
        }

        public static string GetJwt(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<string>(JwtKey);
        }
    }
}