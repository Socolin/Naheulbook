using BoDi;
using Newtonsoft.Json.Linq;
using Socolin.ANSITerminalColor;
using Socolin.TestUtils.JsonComparer;
using Socolin.TestUtils.JsonComparer.Color;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init;

[Binding]
public class JsonComparerInitializer(IObjectContainer objectContainer, ScenarioContext scenarioContext)
{
    [BeforeScenario]
    public void InitializeJsonComparer()
    {
        var jsonColorOptions = new JsonComparerColorOptions
        {
            ColorizeDiff = true,
            ColorizeJson = true,
            Theme = new JsonComparerColorTheme
            {
                DiffAddition = AnsiColor.Background(TerminalRgbColor.FromHex("21541A")),
                DiffDeletion = AnsiColor.Background(TerminalRgbColor.FromHex("542822")),
            },
        };
        objectContainer.RegisterInstanceAs(jsonColorOptions, typeof(JsonComparerColorOptions));
        var jsonComparer = JsonComparer.GetDefault((name, value) =>
        {
            switch (value.Type)
            {
                case JTokenType.Integer:
                    scenarioContext[name] = value.ToObject<int>();
                    break;
                case JTokenType.Float:
                    scenarioContext[name] = value.ToObject<float>();
                    break;
                case JTokenType.String:
                    scenarioContext[name] = value.ToObject<string>();
                    break;
                case JTokenType.Boolean:
                    scenarioContext[name] = value.ToObject<bool>();
                    break;
                default:
                    scenarioContext[name] = value;
                    break;
            }
        }, colorOptions: jsonColorOptions);
        objectContainer.RegisterInstanceAs(jsonComparer, typeof(IJsonComparer));
    }
}