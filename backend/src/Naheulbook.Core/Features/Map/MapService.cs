using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;
using Serilog;

namespace Naheulbook.Core.Features.Map;

public interface IMapService
{
    Task<MapEntity> GetMapAsync(int mapId, int? userId);
    Task<MapEntity> CreateMapAsync(NaheulbookExecutionContext executionContext, CreateMapRequest request, Stream imageStream);
    Task<MapEntity> UpdateMapAsync(NaheulbookExecutionContext executionContext, int mapId, CreateMapRequest request);
    Task<MapLayerEntity> CreateMapLayerAsync(NaheulbookExecutionContext executionContext, int mapId, MapLayerRequest request);
    Task<MapLayerEntity> EditMapLayerAsync(NaheulbookExecutionContext executionContext, int mapLayerId, MapLayerRequest request);
    Task<MapMarkerEntity> CreateMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapLayerId, MapMarkerRequest request);
    Task DeleteMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapMarkerId);
    Task<MapMarkerEntity> EditMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapMarkerId, MapMarkerRequest request);
    Task DeleteMapLayerAsync(NaheulbookExecutionContext executionContext, int mapLayerId);
    Task<List<MapEntity>> GetMapsAsync();
    Task<MapMarkerLinkEntity> CreateMapMarkerLinkAsync(NaheulbookExecutionContext executionContext, int mapMarkerId, MapMarkerLinkRequest request);
    Task<MapMarkerLinkEntity> EditMapMarkerLinkAsync(NaheulbookExecutionContext executionContext, int mapMarkerLinkId, MapMarkerLinkRequest request);
    Task DeleteMapMarkerLinkAsync(NaheulbookExecutionContext executionContext, int mapMarkerLinkId);
}

public class MapService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IJsonUtil jsonUtil,
    IAuthorizationUtil authorizationUtil,
    IMapImageUtil mapImageUtil,
    ILogger logger
) : IMapService
{
    public async Task<MapEntity> GetMapAsync(int mapId, int? userId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var map = await uow.Maps.GetMapDetailsForCurrentUserAsync(mapId, userId);
        if (map == null)
            throw new MapNotFoundException(mapId);

        return map;
    }

    public async Task<MapEntity> CreateMapAsync(NaheulbookExecutionContext executionContext, CreateMapRequest request, Stream imageStream)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var mapData = new MapData
        {
            IsGm = request.Data.IsGm,
            PixelPerUnit = request.Data.PixelPerUnit,
            UnitName = request.Data.UnitName,
            Attribution = request.Data.Attribution
                .Select(x => new MapData.MapAttribution {Name = x.Name, Url = x.Url})
                .ToList(),
        };
        var map = new MapEntity
        {
            Name = request.Name,
            Data = jsonUtil.SerializeNonNull(mapData),
            ImageData = "{}",
            Layers = new List<MapLayerEntity>(),
        };

        uow.Maps.Add(map);
        await uow.SaveChangesAsync();
        map.Data = jsonUtil.SerializeNonNull(mapData);

        try
        {
            var mapImageData = mapImageUtil.SplitMapImage(imageStream, map.Id);
            map.ImageData = jsonUtil.SerializeNonNull(mapImageData);
        }
        catch (Exception ex)
        {
            logger.Warning(ex, "Failed to process map image");
            uow.Maps.Remove(map);
        }

        await uow.SaveChangesAsync();

        return map;
    }

    public async Task<MapEntity> UpdateMapAsync(NaheulbookExecutionContext executionContext, int mapId, CreateMapRequest request)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var map = await uow.Maps.GetAsync(mapId);
        if (map == null)
            throw new MapNotFoundException(mapId);

        map.Name = request.Name;
        map.Data = jsonUtil.SerializeNonNull(request.Data);

        await uow.SaveChangesAsync();

        return map;
    }

    public async Task<MapLayerEntity> CreateMapLayerAsync(NaheulbookExecutionContext executionContext, int mapId, MapLayerRequest request)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            switch (request.Source)
            {
                case "official":
                    await authorizationUtil.EnsureAdminAccessAsync(executionContext);
                    break;
                case "private":
                    break;
                default:
                    throw new InvalidSourceException(request.Source);
            }

            var map = await uow.Maps.GetAsync(mapId);
            if (map == null)
                throw new MapNotFoundException(mapId);

            var mapLayer = new MapLayerEntity
            {
                Name = request.Name,
                Source = request.Source,
                IsGm = request.IsGm,
                MapId = mapId,
                UserId = request.Source == "official" ? null : executionContext.UserId,
            };

            uow.MapLayers.Add(mapLayer);
            await uow.SaveChangesAsync();

            return mapLayer;
        }
    }

    public async Task<MapLayerEntity> EditMapLayerAsync(NaheulbookExecutionContext executionContext, int mapLayerId, MapLayerRequest request)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var mapLayer = await uow.MapLayers.GetAsync(mapLayerId);
            if (mapLayer == null)
                throw new MapLayerNotFoundException(mapLayerId);

            switch (mapLayer.Source)
            {
                case "official":
                    await authorizationUtil.EnsureAdminAccessAsync(executionContext);
                    break;
                case "private":
                    break;
                default:
                    throw new InvalidSourceException(request.Source);
            }

            switch (request.Source)
            {
                case "official":
                    await authorizationUtil.EnsureAdminAccessAsync(executionContext);
                    break;
                case "private":
                    break;
                default:
                    throw new InvalidSourceException(request.Source);
            }

            mapLayer.Name = request.Name;
            mapLayer.Source = request.Source;
            mapLayer.IsGm = request.IsGm;
            mapLayer.UserId = request.Source == "official" ? null : executionContext.UserId;

            await uow.SaveChangesAsync();
            await uow.MapLayers.LoadMarkersForResponseAsync(mapLayer);

            return mapLayer;
        }
    }

    public async Task<MapMarkerEntity> CreateMapMarkerAsync(
        NaheulbookExecutionContext executionContext,
        int mapLayerId,
        MapMarkerRequest request
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var mapLayer = await uow.MapLayers.GetAsync(mapLayerId);
        if (mapLayer == null)
            throw new MapLayerNotFoundException(mapLayerId);

        await authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapLayer);

        var mapMarker = new MapMarkerEntity
        {
            LayerId = mapLayerId,
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            MarkerInfo = jsonUtil.SerializeNonNull(request.MarkerInfo),
            Links = new List<MapMarkerLinkEntity>(),
        };

        uow.MapMarkers.Add(mapMarker);

        await uow.SaveChangesAsync();

        return mapMarker;
    }

    public async Task DeleteMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapMarkerId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var mapMarker = await uow.MapMarkers.GetWithLayerAsync(mapMarkerId);
        if (mapMarker == null)
            throw new MapMarkerNotFoundException(mapMarkerId);

        await authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapMarker.Layer);

        uow.MapMarkers.Remove(mapMarker);

        await uow.SaveChangesAsync();
    }

    public async Task<MapMarkerEntity> EditMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapMarkerId, MapMarkerRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var mapMarker = await uow.MapMarkers.GetWithLayerAsync(mapMarkerId);
        if (mapMarker == null)
            throw new MapMarkerNotFoundException(mapMarkerId);

        await authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapMarker.Layer);

        mapMarker.Name = request.Name;
        mapMarker.Description = request.Description;
        mapMarker.Type = request.Type;
        mapMarker.MarkerInfo = jsonUtil.SerializeNonNull(request.MarkerInfo);

        await uow.SaveChangesAsync();
        await uow.MapMarkers.LoadLinksAsync(mapMarker);

        return mapMarker;
    }

    public async Task DeleteMapLayerAsync(NaheulbookExecutionContext executionContext, int mapLayerId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var mapLayer = await uow.MapLayers.GetAsync(mapLayerId);
        if (mapLayer == null)
            throw new MapLayerNotFoundException(mapLayerId);

        await authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapLayer);

        uow.MapLayers.Remove(mapLayer);

        await uow.SaveChangesAsync();
    }

    public async Task<List<MapEntity>> GetMapsAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var maps = await uow.Maps.GetAllAsync();

        return maps;
    }

    public async Task<MapMarkerLinkEntity> CreateMapMarkerLinkAsync(
        NaheulbookExecutionContext executionContext,
        int mapMarkerId,
        MapMarkerLinkRequest request
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var mapMarker = await uow.MapMarkers.GetWithLayerAsync(mapMarkerId);
        if (mapMarker == null)
            throw new MapMarkerNotFoundException(mapMarkerId);

        await authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapMarker.Layer);

        var targetMap = await uow.Maps.GetAsync(request.TargetMapId);
        if (targetMap == null)
            throw new MapNotFoundException(request.TargetMapId);

        if (request.TargetMapMarkerId.HasValue)
        {
            var targetMapMarker = await uow.MapMarkers.GetWithLayerAsync(request.TargetMapMarkerId.Value);
            if (targetMapMarker == null)
                throw new MapMarkerNotFoundException(request.TargetMapMarkerId.Value);

            await authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, targetMapMarker.Layer);
        }

        var mapMarkerLink = new MapMarkerLinkEntity
        {
            Name = request.Name,
            MapMarkerId = mapMarkerId,
            TargetMap = targetMap,
            TargetMapId = request.TargetMapId,
            TargetMapMarkerId = request.TargetMapMarkerId,
        };

        uow.MapMarkerLinks.Add(mapMarkerLink);

        await uow.SaveChangesAsync();

        return mapMarkerLink;
    }

    public async Task<MapMarkerLinkEntity> EditMapMarkerLinkAsync(NaheulbookExecutionContext executionContext, int mapMarkerLinkId, MapMarkerLinkRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var mapMarkerLink = await uow.MapMarkerLinks.GetWithLayerAsync(mapMarkerLinkId);
        if (mapMarkerLink == null)
            throw new MapMarkerNotFoundException(mapMarkerLinkId);

        await authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapMarkerLink.MapMarker.Layer);

        mapMarkerLink.Name = request.Name;
        mapMarkerLink.TargetMapId = request.TargetMapId;
        mapMarkerLink.TargetMapMarkerId = request.TargetMapMarkerId;

        await uow.SaveChangesAsync();

        await uow.MapMarkerLinks.LoadTargetMapAsync(mapMarkerLink);

        return mapMarkerLink;
    }

    public async Task DeleteMapMarkerLinkAsync(NaheulbookExecutionContext executionContext, int mapMarkerLinkId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var mapMarkerLink = await uow.MapMarkerLinks.GetWithLayerAsync(mapMarkerLinkId);
        if (mapMarkerLink == null)
            throw new MapMarkerNotFoundException(mapMarkerLinkId);

        await authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapMarkerLink.MapMarker.Layer);

        uow.MapMarkerLinks.Remove(mapMarkerLink);

        await uow.SaveChangesAsync();
    }
}