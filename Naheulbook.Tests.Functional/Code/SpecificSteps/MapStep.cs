using System.Collections.Generic;
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
            var targetMap = _testDataUtil.AddMap().GetLast<MapEntity>();
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddMap(x =>
                {
                    x.Layers = new[]
                    {
                        new MapLayerEntity
                        {
                            Name = "some-layer-name",
                            Source = "official",
                            IsGm = true,
                            Markers = new List<MapMarkerEntity>
                            {
                                new MapMarkerEntity
                                {
                                    Name = "some-marker-name",
                                    Description = "some-marker-description",
                                    MarkerInfo = "{}",
                                    Type = "point",
                                    Links = new List<MapMarkerLinkEntity>
                                    {
                                        new MapMarkerLinkEntity
                                        {
                                            Name = "some-link-name",
                                            TargetMapId = targetMap.Id
                                        }
                                    }
                                }
                            }
                        }
                    };
                });
        }

        [Given(@"(a|\d+) maps? with a marker")]
        public void GivenXMapWithAMarker(string amount)
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddMap(x =>
                {
                    x.Layers = new[]
                    {
                        new MapLayerEntity
                        {
                            Name = "some-layer-name",
                            Source = "official",
                            IsGm = true,
                            Markers = new List<MapMarkerEntity>
                            {
                                new MapMarkerEntity
                                {
                                    Name = "some-marker-name",
                                    Description = "some-marker-description",
                                    MarkerInfo = "{}",
                                    Type = "point",
                                    Links = new List<MapMarkerLinkEntity>()
                                }
                            }
                        }
                    };
                });
        }

        [Given(@"(a|\d+) maps? with a layer")]
        public void GivenXMapWithALayer(string amount)
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddMap(x =>
                {
                    x.Layers = new[]
                    {
                        new MapLayerEntity
                        {
                            Name = "some-layer-name",
                            Source = "official"
                        }
                    };
                });
        }
    }
}