using System.Collections.Generic;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class MapStep(TestDataUtil testDataUtil)
{
    [Given(@"^(a|\d+) maps?$")]
    public void GivenXMap(string amount)
    {
        for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            testDataUtil.AddMap();
    }

    [Given(@"^(a|\d+) maps? with all data$")]
    public void GivenXMapWithAllData(string amount)
    {
        var targetMap = testDataUtil.AddMap().GetLast<MapEntity>();
        for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            testDataUtil.AddMap(x =>
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
                            new()
                            {
                                Name = "some-marker-name",
                                Description = "some-marker-description",
                                MarkerInfo = "{}",
                                Type = "point",
                                Links = new List<MapMarkerLinkEntity>
                                {
                                    new()
                                    {
                                        Name = "some-link-name",
                                        TargetMapId = targetMap.Id,
                                    },
                                },
                            },
                        },
                    },
                };
            });
    }

    [Given(@"^(a|\d+) maps? with a marker$")]
    public void GivenXMapWithAMarker(string amount)
    {
        for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            testDataUtil.AddMap(x =>
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
                            new()
                            {
                                Name = "some-marker-name",
                                Description = "some-marker-description",
                                MarkerInfo = "{}",
                                Type = "point",
                                Links = new List<MapMarkerLinkEntity>(),
                            },
                        },
                    },
                };
            });
    }

    [Given(@"^(a|\d+) maps? with a layer$")]
    public void GivenXMapWithALayer(string amount)
    {
        for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            testDataUtil.AddMap(x =>
            {
                x.Layers = new[]
                {
                    new MapLayerEntity
                    {
                        Name = "some-layer-name",
                        Source = "official",
                    },
                };
            });
    }
}