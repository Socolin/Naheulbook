using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class MapStep
    {
        private readonly TestDataUtil _testDataUtil;

        public MapStep(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given(@"(a|\d+) maps?")]
        public void GivenXMap(string amount)
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddMap();
        }

        [Given(@"(a|\d+) maps? with all data")]
        public void GivenXMapWithAllData(string amount)
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddMap(x =>
                {
                    x.Layers = new []
                    {
                        new MapLayer
                        {
                            Name = "some-layer-name",
                            Source = "official"
                        }
                    };
                });
        }
    }
}