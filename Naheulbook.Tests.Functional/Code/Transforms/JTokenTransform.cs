using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Naheulbook.Tests.Functional.Code.Extensions;
using Naheulbook.TestUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Transforms;

public class JTokenTransform
{
    [Binding]
    public class StringReplacementTransform
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly TestDataUtil _testDataUtil;

        public StringReplacementTransform(ScenarioContext scenarioContext, TestDataUtil testDataUtil)
        {
            _scenarioContext = scenarioContext;
            _testDataUtil = testDataUtil;
        }

        [StepArgumentTransformation]
        public JToken ReplaceJToken(string input)
        {
            var expandedJson = input.ExecuteReplacement(_scenarioContext, _testDataUtil);
            try
            {
                return JsonConvert.DeserializeObject<JToken>(expandedJson, new JsonSerializerSettings()
                {
                    DateParseHandling = DateParseHandling.None
                });
            }
            catch (JsonReaderException ex)
            {
                var splitJson = expandedJson.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

                var showErrorOutput = new StringBuilder();
                var contextSize = 3;
                for (var i = -contextSize; i <= contextSize; i++)
                {
                    if (ex.LineNumber + i < 1)
                        continue;
                    if (ex.LineNumber + i >= splitJson.Length)
                        continue;

                    var line = splitJson[ex.LineNumber - 1 + i];
                    showErrorOutput.Append((ex.LineNumber + i).ToString().PadRight(4, ' ')).Append('|');
                    showErrorOutput.AppendLine(line);
                    if (i == 0)
                    {
                        showErrorOutput.AppendLine("".PadLeft(ex.LinePosition + 5) + "^-- " + ex.Message);
                    }
                }

                var stackTraceScenarioFrame = new StackTrace(true).GetFrames().FirstOrDefault(x => x.GetFileName()?.EndsWith(".feature") == true);

                throw new Exception($"During the scenario '{_scenarioContext.ScenarioInfo.Title}'\nwhile parsing step `{_scenarioContext.StepContext.StepInfo.Text}`" +
                                    $"\nInvalid JSON found. At line {ex.LineNumber} at position: {ex.LinePosition}" +
                                    $"\n\n{showErrorOutput}" +
                                    $"\n\nIn file {stackTraceScenarioFrame?.GetFileName()}:{stackTraceScenarioFrame?.GetFileLineNumber()}\n\n"
                    , ex);
            }
        }
    }
}