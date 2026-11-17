using AwesomeAssertions;
using Naheulbook.Core.Features.Map;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Map;

[TestFixture]
[TestOf(typeof(MapMapper))]
public class MapMapperTests
{
    private MapMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new MapMapper();
    }

    [Test]
    public void ToResponse_Map_MapsFieldsAndLayers()
    {
        var layer = new MapLayerEntity {Id = 6, Name = "some-layer", Source = "some-source", Markers = []};
        var map = new MapEntity
        {
            Id = 3,
            Name = "some-name",
            Data = """{"unitName":"km","pixelPerUnit":2,"isGm":true}""",
            ImageData = """{"zoomCount":3,"width":100,"height":200,"extraZoomCount":1}""",
            Layers = [layer],
        };

        var actual = _mapper.ToResponse(map);

        actual.Should().BeEquivalentTo(new MapResponse
        {
            Id = 3,
            Name = "some-name",
            Data = new MapData {UnitName = "km", PixelPerUnit = 2, IsGm = true},
            ImageData = new MapImageData {ZoomCount = 3, Width = 100, Height = 200, ExtraZoomCount = 1},
            Layers = [_mapper.ToResponse(layer)],
        });
    }

    [Test]
    public void ToSummaryResponse_Map_MapsIdNameAndData()
    {
        var map = new MapEntity
        {
            Id = 3,
            Name = "some-name",
            Data = """{"unitName":"km","isGm":true}""",
            ImageData = "{}",
        };

        var actual = _mapper.ToSummaryResponse(map);

        actual.Should().BeEquivalentTo(new MapSummaryResponse
        {
            Id = 3,
            Name = "some-name",
            Data = new MapData {UnitName = "km", IsGm = true},
        });
    }

    [Test]
    public void ToResponse_MapLayer_MapsFieldsAndMarkers()
    {
        var marker = new MapMarkerEntity {Id = 8, Name = "some-marker", Type = "some-type", MarkerInfo = "{}", Links = []};
        var layer = new MapLayerEntity
        {
            Id = 3,
            Name = "some-name",
            Source = "some-source",
            IsGm = true,
            MapId = 4,
            Markers = [marker],
        };

        var actual = _mapper.ToResponse(layer);

        actual.Should().BeEquivalentTo(new MapLayerResponse
        {
            Id = 3,
            Name = "some-name",
            Source = "some-source",
            IsGm = true,
            Markers = [_mapper.ToResponse(marker)],
        });
    }

    [Test]
    public void ToResponse_MapLayerWithoutMarkers_ReturnsEmptyMarkers()
    {
        var layer = new MapLayerEntity {Name = "some-name", Source = "some-source", Markers = null!};

        var actual = _mapper.ToResponse(layer);

        actual.Markers.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToResponse_MapMarker_MapsFieldsAndLinks()
    {
        var link = new MapMarkerLinkEntity {Id = 9, TargetMapId = 2, TargetMap = new MapEntity {Name = "some-target", Data = "{}"}};
        var marker = new MapMarkerEntity
        {
            Id = 3,
            Name = "some-name",
            Description = "some-description",
            Type = "some-type",
            MarkerInfo = """{"x":1}""",
            LayerId = 4,
            Links = [link],
        };

        var actual = _mapper.ToResponse(marker);

        actual.Should().BeEquivalentTo(new MapMarkerResponse
        {
            Id = 3,
            Name = "some-name",
            Description = "some-description",
            Type = "some-type",
            MarkerInfo = new JObject {["x"] = 1},
            Links = [_mapper.ToResponse(link)],
        });
    }

    [Test]
    public void ToResponse_MapMarkerWithEmptyMarkerInfo_ReturnsEmptyMarkerInfo()
    {
        var marker = new MapMarkerEntity {Name = "some-name", Type = "some-type", MarkerInfo = "", Links = []};

        var actual = _mapper.ToResponse(marker);

        actual.MarkerInfo.Should().BeEquivalentTo(new JObject());
    }

    [Test]
    public void ToResponse_MapMarkerLink_MapsFieldsAndTargetMapInfo()
    {
        var link = new MapMarkerLinkEntity
        {
            Id = 3,
            Name = "some-name",
            MapMarkerId = 4,
            TargetMapId = 5,
            TargetMapMarkerId = 6,
            TargetMap = new MapEntity {Name = "some-target-map", Data = """{"isGm":true}"""},
        };

        var actual = _mapper.ToResponse(link);

        actual.Should().BeEquivalentTo(new MapMarkerLinkResponse
        {
            Id = 3,
            Name = "some-name",
            TargetMapName = "some-target-map",
            TargetMapId = 5,
            TargetMapIsGm = true,
            TargetMapMarkerId = 6,
        });
    }
}
