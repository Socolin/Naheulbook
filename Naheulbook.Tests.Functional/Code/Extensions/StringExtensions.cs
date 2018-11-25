using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions
{
    public static class StringExtensions
    {
        public static string ExecuteReplacement(this string str, ScenarioContext context)
        {
            foreach (var contextKey in context.Keys)
            {
                str = str.Replace($"${{{contextKey}}}", context[contextKey].ToString());
            }

            return str;
        }
    }
}