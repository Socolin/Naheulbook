using BoDi;
using Newtonsoft.Json.Linq;
using Socolin.ANSITerminalColor;
using Socolin.TestUtils.JsonComparer;
using Socolin.TestUtils.JsonComparer.Color;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init;

[Binding]
public class JsonComparerInitializer
{
    private readonly IObjectContainer _objectContainer;
    private readonly ScenarioContext _scenarioContext;

    public JsonComparerInitializer(IObjectContainer objectContainer, ScenarioContext scenarioContext)
    {
        _objectContainer = objectContainer;
        _scenarioContext = scenarioContext;
    }

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
            }
        };
        _objectContainer.RegisterInstanceAs(jsonColorOptions, typeof(JsonComparerColorOptions));
        var jsonComparer = JsonComparer.GetDefault((name, value) =>
        {
            switch (value.Type)
            {
                case JTokenType.Integer:
                    _scenarioContext[name] = value.ToObject<int>();
                    break;
                case JTokenType.Float:
                    _scenarioContext[name] = value.ToObject<float>();
                    break;
                case JTokenType.String:
                    _scenarioContext[name] = value.ToObject<string>();
                    break;
                case JTokenType.Boolean:
                    _scenarioContext[name] = value.ToObject<bool>();
                    break;
                default:
                    _scenarioContext[name] = value;
                    break;
            }
        }, colorOptions: jsonColorOptions);
        _objectContainer.RegisterInstanceAs(jsonComparer, typeof(IJsonComparer));
    }
}