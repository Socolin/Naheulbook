using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class LocationSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public LocationSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given(@"(a|\d+) locations?")]
        public void GivenXLocation(string amount)
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddLocation();
        }

        [Given("a location with a map and a parent")]
        public void GivenALocationWithAMapAndAParent()
        {
            _testDataUtil.AddLocation();
            _testDataUtil.AddLocation(l =>
            {
                l.Parent = _testDataUtil.GetLast<Location>();
                l.Maps = new List<LocationMap>
                {
                    new LocationMap
                    {
                        Data = "{}",
                        File = "some-file",
                        Name = "some-map-name",
                        IsGm = true,
                    }
                };
            });
        }
    }
}