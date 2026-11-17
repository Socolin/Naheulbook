using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Features.Map;

public interface IMapMapper
{
    MapResponse ToResponse(MapEntity map);
    MapSummaryResponse ToSummaryResponse(MapEntity map);
    MapLayerResponse ToResponse(MapLayerEntity mapLayer);
    MapMarkerResponse ToResponse(MapMarkerEntity mapMarker);
    MapMarkerLinkResponse ToResponse(MapMarkerLinkEntity mapMarkerLink);
}

public class MapMapper : IMapMapper
{
    public MapResponse ToResponse(MapEntity map)
    {
        return new MapResponse
        {
            Id = map.Id,
            Name = map.Name,
            Data = MapperHelpers.FromJson<MapData>(map.Data)!,
            ImageData = MapperHelpers.FromJson<MapImageData>(map.ImageData)!,
            Layers = map.Layers.Select(ToResponse).ToList(),
        };
    }

    public MapSummaryResponse ToSummaryResponse(MapEntity map)
    {
        return new MapSummaryResponse
        {
            Id = map.Id,
            Name = map.Name,
            Data = MapperHelpers.FromJson<MapData>(map.Data)!,
        };
    }

    public MapLayerResponse ToResponse(MapLayerEntity mapLayer)
    {
        return new MapLayerResponse
        {
            Id = mapLayer.Id,
            Name = mapLayer.Name,
            Source = mapLayer.Source,
            IsGm = mapLayer.IsGm,
            Markers = mapLayer.Markers?.Select(ToResponse).ToList() ?? [],
        };
    }

    public MapMarkerResponse ToResponse(MapMarkerEntity mapMarker)
    {
        return new MapMarkerResponse
        {
            Id = mapMarker.Id,
            Name = mapMarker.Name,
            Description = mapMarker.Description,
            Type = mapMarker.Type,
            MarkerInfo = MapperHelpers.FromJsonNotNull<JObject>(mapMarker.MarkerInfo),
            Links = mapMarker.Links.Select(ToResponse).ToList(),
        };
    }

    public MapMarkerLinkResponse ToResponse(MapMarkerLinkEntity mapMarkerLink)
    {
        return new MapMarkerLinkResponse
        {
            Id = mapMarkerLink.Id,
            Name = mapMarkerLink.Name,
            TargetMapName = mapMarkerLink.TargetMap.Name,
            TargetMapId = mapMarkerLink.TargetMapId,
            TargetMapIsGm = MapperHelpers.FromJsonNotNull<MapData>(mapMarkerLink.TargetMap.Data).IsGm,
            TargetMapMarkerId = mapMarkerLink.TargetMapMarkerId,
        };
    }
}
