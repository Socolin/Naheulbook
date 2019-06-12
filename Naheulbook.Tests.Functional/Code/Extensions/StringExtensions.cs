using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions
{
    public static class StringExtensions
    {
        public static string ExecuteReplacement(this string str, ScenarioContext context, TestDataUtil testDataUtil)
        {
            var replacementTokens = ListReplacementTokens(str);

            foreach (var replacementToken in replacementTokens)
            {
                try
                {
                    var value = GetValue(str, replacementToken, context, testDataUtil);
                    string replacementValue;
                    switch (value)
                    {
                        case null:
                            replacementValue = "null";
                            break;
                        case bool _:
                            replacementValue = value.ToString().ToLowerInvariant();
                            break;
                        default:
                            replacementValue = value.ToString();
                            break;
                    }

                    str = str.Replace($"${{{replacementToken}}}", replacementValue);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An exception occured while replacing '{replacementToken}': {ex.Message}", ex);
                }
            }

            return str;
        }

        private static object GetValue(string str, string replacementToken, ScenarioContext context, TestDataUtil testDataUtil)
        {
            if (replacementToken.Contains("."))
            {
                var replacements = replacementToken.Split('.');
                object rootValue;
                if (context.Keys.Contains(replacements.First()))
                    rootValue = context[replacements.First()];
                else if (IsArrayAccessor(replacements.ElementAt(1)))
                    rootValue = testDataUtil.GetAllByTypeName(replacements.First());
                else
                    rootValue = testDataUtil.GetByTypeName(replacements.First());

                if (rootValue == null)
                    throw new Exception($"Failed to find key {replacementToken} in ScenarioContext or TestData when replacing '{str}'");

                var propertiesNames = replacements.Skip(1).ToList();
                return GetProperty(rootValue, propertiesNames);
            }

            if (context.Keys.Contains(replacementToken))
                return context[replacementToken].ToString();

            var value = testDataUtil.GetByTypeName(replacementToken);
            if (value != null)
                return value;

            throw new Exception($"Failed to find key {replacementToken} in ScenarioContext or TestData when replacing '{str}'");
        }


        private static object GetProperty(object rootValue, List<string> propertiesNames)
        {
            var obj = rootValue;
            try
            {
                foreach (var propertyName in propertiesNames)
                {
                    if (IsArrayAccessor(propertyName))
                    {
                        var arrayIndex = int.Parse(propertyName.Substring(1, propertyName.Length - 2));
                        if (arrayIndex < 0)
                        {
                            obj = (obj as IEnumerable)?.OfType<object>().Reverse().ElementAt(-arrayIndex - 1);
                        }
                        else
                        {
                            obj = (obj as IEnumerable)?.OfType<object>().ElementAt(arrayIndex);
                        }
                    }
                    else
                    {
                        obj = obj.GetType().GetProperty(propertyName).GetValue(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to find property `{string.Join('.', propertiesNames)}` in object.", ex);
            }

            return obj;
        }

        private static bool IsArrayAccessor(string propertyName)
        {
            return propertyName.StartsWith("[") && propertyName.EndsWith("]");
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