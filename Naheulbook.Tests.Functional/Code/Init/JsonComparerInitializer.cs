using BoDi;
using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.JsonComparer;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Init
{
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
        public void InitializeIoc()
        {
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
            });
            _objectContainer.RegisterInstanceAs(jsonComparer, typeof(IJsonComparer));
        }
    }
}