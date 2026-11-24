using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddMap(Action<MapEntity> customizer = null)
    {
        var map = new MapEntity
        {
            Name = RngUtil.GetRandomString("some-map-name"),
            ImageData = JsonConvert.SerializeObject(new MapImageData
                {
                    Width = 40,
                    Height = 20,
                    ZoomCount = 5,
                    ExtraZoomCount = 1,
                },
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                }
            ),
            Data = JsonConvert.SerializeObject(new MapData
                {
                    IsGm = true,
                    Attribution =
                    [
                        new MapData.MapAttribution
                        {
                            Name = "some-attribution-name",
                            Url = "some-attribution-url",
                        },
                    ],
                },
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                }
            ),
        };

        return SaveEntity(map, customizer);
    }

    public TestDataUtil AddMapLayer(Action<MapLayerEntity> customizer = null)
    {
        return AddMapLayer(out _, customizer);
    }

    public TestDataUtil AddMapLayer(out MapLayerEntity mapLayer, Action<MapLayerEntity> customizer = null)
    {
        mapLayer = new MapLayerEntity
        {
            Name = RngUtil.GetRandomString("some-map-layer-name"),
            Source = "official",
            IsGm = false,
            MapId = GetLast<MapEntity>().Id,
        };

        return SaveEntity(mapLayer, customizer);
    }

    public TestDataUtil AddMapMarker(Action<MapMarkerEntity> customizer = null)
    {
        return AddMapMarker(out _, customizer);
    }

    public TestDataUtil AddMapMarker(out MapMarkerEntity mapMarker, Action<MapMarkerEntity> customizer = null)
    {
        mapMarker = new MapMarkerEntity
        {
            Name = RngUtil.GetRandomString("some-map-marker-name"),
            Type = "point",
            MarkerInfo = "{}",
            LayerId = GetLast<MapLayerEntity>().Id,
        };

        return SaveEntity(mapMarker, customizer);
    }

    public TestDataUtil AddMapMarkerLink(MapMarkerEntity targetMapMarker, Action<MapMarkerLinkEntity> customizer = null)
    {
        return AddMapMarkerLink(out _, targetMapMarker, customizer);
    }

    public TestDataUtil AddMapMarkerLink(out MapMarkerLinkEntity mapMarkerLink, MapMarkerEntity targetMapMarker, Action<MapMarkerLinkEntity> customizer = null)
    {
        mapMarkerLink = new MapMarkerLinkEntity
        {
            Name = RngUtil.GetRandomString("some-map-marker-link-name"),
            MapMarkerId = GetLast<MapMarkerEntity>().Id,
            TargetMapId = GetLast<MapEntity>().Id,
            TargetMapMarkerId = targetMapMarker.Id,
        };

        return SaveEntity(mapMarkerLink, customizer);
    }
}
