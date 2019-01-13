using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions
{
    public static class StringExtensions
    {
        public static string ExecuteReplacement(this string str, ScenarioContext context)
        {
            var replacementTokens = ListReplacementTokens(str);

            foreach (var replacementToken in replacementTokens)
            {
                string replacementValue;
                if (replacementToken.Contains("."))
                {
                    var replacements = replacementToken.Split('.');
                    if (!context.Keys.Contains(replacements.First()))
                        throw new Exception($"Failed to find key {replacements.First()} in scenario context when replacing '{str}'");
                    var rootValue = context[replacements.First()];
                    replacementValue = GetProperty(rootValue, replacements.Skip(1).ToList());
                }
                else
                {
                    if (!context.Keys.Contains(replacementToken))
                        throw new Exception($"Failed to find key {replacementToken} in scenario context when replacing '{str}'");
                    replacementValue = context[replacementToken].ToString();
                }

                str = str.Replace($"${{{replacementToken}}}", replacementValue);
            }

            return str;
        }

        private static string GetProperty(object rootValue, List<string> propertiesNames)
        {
            var obj = rootValue;
            try
            {
                foreach (var propertyName in propertiesNames)
                {
                    obj = obj.GetType().GetProperty(propertyName).GetValue(obj);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to find property `{string.Join('.', propertiesNames.Skip(1))}` in object.", ex);
            }

            return obj.ToString();
        }

        private static IEnumerable<string> ListReplacementTokens(string str)
        {
            var currentToken = new StringBuilder();
            var inReplacement = false;
            var previousChar = '\0';
            var tokens = new List<string>();

            foreach (var c in str)
            {
                if (previousChar == '$' && c == '{')
                {
                    inReplacement = true;
                    currentToken.Clear();
                }
                else if (inReplacement && c == '}')
                {
                    tokens.Add(currentToken.ToString());
                    inReplacement = false;
                }
                else if (inReplacement)
                    currentToken.Append(c);

                previousChar = c;
            }

            return tokens;
        }
    }
}