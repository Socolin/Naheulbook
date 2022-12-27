using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naheulbook.TestUtils;
using Socolin.ANSITerminalColor;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions;

public static class StringExtensions
{
    private static readonly AnsiColor KeyColor = AnsiColor.Foreground(Terminal256ColorCodes.AquaC14);
    private static readonly AnsiColor ErrorColor = AnsiColor.Foreground(Terminal256ColorCodes.RedC9);

    public static string ExecuteReplacement(this string str, ScenarioContext context, TestDataUtil testDataUtil)
    {
        var replacementTokens = ListReplacementTokens(str);
        var missingKeys = new List<string>();

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
                        replacementValue = value.ToString()?.ToLowerInvariant();
                        break;
                    default:
                        replacementValue = value.ToString();
                        break;
                }

                str = str.Replace($"${{{replacementToken}}}", replacementValue);
            }
            catch (ReplacementKeyNotfoundException ex)
            {
                missingKeys.Add(ex.Key);
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception occured while replacing '{replacementToken}': {ex.Message}", ex);
            }
        }


        if (missingKeys.Count > 0)
        {
            var strWithError = str;
            foreach (var key in missingKeys)
            {
                strWithError = strWithError.Replace($"${{{key}}}", ErrorColor.Colorize($"${{{key}}}"));
            }

            throw new Exception($"Failed to find key '{string.Join(",", missingKeys.Distinct().Select(x => KeyColor.Colorize(x)))}' in ScenarioContext or TestData when replacing:\n{strWithError}\n");
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
                throw new ReplacementKeyNotfoundException(replacementToken);

            var propertiesNames = replacements.Skip(1).ToList();
            return GetProperty(rootValue, propertiesNames);
        }

        if (context.Keys.Contains(replacementToken))
            return context[replacementToken].ToString();

        var value = testDataUtil.GetByTypeName(replacementToken);
        if (value != null)
            return value;

        throw new ReplacementKeyNotfoundException(replacementToken);
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
                    var propertyInfo = obj?.GetType().GetProperty(propertyName);
                    if (propertyInfo == null)
                        throw new Exception($"Failed to find property `{string.Join(".", propertiesNames)}` in object.");
                    obj = propertyInfo?.GetValue(obj);
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

    private class ReplacementKeyNotfoundException : Exception
    {
        public string Key { get; }

        public ReplacementKeyNotfoundException(string key)
        {
            Key = key;
        }
    }
}