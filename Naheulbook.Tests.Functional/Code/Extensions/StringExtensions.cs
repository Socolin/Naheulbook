using System;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions
{
    public static class StringExtensions
    {
        public static string ExecuteReplacement(this string str, ScenarioContext context)
        {
            foreach (var contextKey in context.Keys)
                str = str.Replace($"${{{contextKey}}}", context[contextKey].ToString());

            var match = Regex.Match(str, "\\${(?<Key>[a-z0-9A-Z_-]+)}");
            if (match.Success)
                throw new Exception($"Failed to key {match.Groups["Key"]} in scenario context when replacing '{str}'");

            return str;
        }
    }
}