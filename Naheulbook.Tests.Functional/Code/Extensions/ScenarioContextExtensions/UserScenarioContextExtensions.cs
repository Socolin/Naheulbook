using System.Net;
using Socolin.TestUtils.FakeSmtp;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;

public static class UserScenarioContextExtensions
{
    private const string LastReceivedMailKey = "LastReceivedMail";
    private const string UsernameKey = "Username";
    private const string PasswordKey = "Password";
    private const string UserIdKey = "UserId";
    private const string ActivationCodeKey = "ActivationCode";
    private const string JwtKey = "Jwt";
    private const string MapImageOutputDirectoryKey = "MapImageOutputDirectory";
    private const string HttpCookiesContainerKey = "HttpCookiesContainer";

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

    public static void SetUserId(this ScenarioContext scenarioContext, int userId)
    {
        scenarioContext.Set(userId, UserIdKey);
    }

    public static int GetUserId(this ScenarioContext scenarioContext)
    {
        return scenarioContext.Get<int>(UserIdKey);
    }

    public static void SetMapImageOutputDirectory(this ScenarioContext scenarioContext, string directory)
    {
        scenarioContext.Set(directory, MapImageOutputDirectoryKey);
    }

    public static string GetMapImageOutputDirectory(this ScenarioContext scenarioContext)
    {
        return scenarioContext.Get<string>(MapImageOutputDirectoryKey);
    }

    public static void SetHttpCookiesContainer(this ScenarioContext scenarioContext, CookieContainer cookieContainer)
    {
        scenarioContext.Set(cookieContainer, HttpCookiesContainerKey);
    }

    public static CookieContainer GetHttpCookiesContainer(this ScenarioContext scenarioContext)
    {
        return scenarioContext.Get<CookieContainer>(HttpCookiesContainerKey);
    }
}