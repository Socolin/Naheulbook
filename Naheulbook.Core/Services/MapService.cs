using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Services
{
    public interface IMapService
    {
        Task<Map> GetMapAsync(int mapId, int? userId);
        Task<Map> CreateMapAsync(NaheulbookExecutionContext executionContext, CreateMapRequest request, Stream imageStream);
        Task<MapLayer> CreateMapLayerAsync(NaheulbookExecutionContext executionContext, int mapId, CreateMapLayerRequest request);
        Task<MapMarker> CreateMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapLayerId, MapMarkerRequest request);
        Task DeleteMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapMarkerId);
        Task<MapMarker> UpdateMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapMarkerId, MapMarkerRequest request);
        Task DeleteMapLayerAsync(NaheulbookExecutionContext executionContext, int mapLayerId);
        Task<List<Map>> GetMapsAsync();
    }

    public class MapService : IMapService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IJsonUtil _jsonUtil;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IMapImageUtil _mapImageUtil;

        public MapService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IJsonUtil jsonUtil,
            IAuthorizationUtil authorizationUtil,
            IMapImageUtil mapImageUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _jsonUtil = jsonUtil;
            _authorizationUtil = authorizationUtil;
            _mapImageUtil = mapImageUtil;
        }

        public async Task<Map> GetMapAsync(int mapId, int? userId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var map = await uow.Maps.GetMapDetailsForCurrentUserAsync(mapId, userId);
                if (map == null)
                    throw new MapNotFoundException(mapId);

                return map;
            }
        }

        public async Task<Map> CreateMapAsync(NaheulbookExecutionContext executionContext, CreateMapRequest request, Stream imageStream)
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var mapData = new MapData
                {
                    PixelPerUnit = request.Data.PixelPerUnit,
                    UnitName = request.Data.UnitName,
                    Attribution = request.Data.Attribution
                        .Select(x => new MapData.MapAttribution {Name = x.Name, Url = x.Url})
                        .ToList()
                };
                var map = new Map
                {
                    Name = request.Name,
                    Data = _jsonUtil.SerializeNonNull(mapData),
                    ImageData = "{}"
                };

                uow.Maps.Add(map);
                await uow.SaveChangesAsync();
                map.Data = _jsonUtil.SerializeNonNull(mapData);

                try
                {
                    var mapImageData = _mapImageUtil.SplitMapImage(imageStream, map.Id);
                    map.ImageData = _jsonUtil.SerializeNonNull(mapImageData);
                }
                catch (Exception)
                {
                    uow.Maps.Remove(map);
                }

                await uow.SaveChangesAsync();

                return map;
            }
        }

        public async Task<MapLayer> CreateMapLayerAsync(NaheulbookExecutionContext executionContext, int mapId, CreateMapLayerRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                switch (request.Source)
                {
                    case "official":
                        await _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                        break;
                    case "private":
                        break;
                    default:
                        throw new InvalidSourceException(request.Source);
                }

                var map = await uow.Maps.GetAsync(mapId);
                if (map == null)
                    throw new MapNotFoundException(mapId);

                var mapLayer = new MapLayer
                {
                    Name = request.Name,
                    Source = request.Source,
                    MapId = mapId,
                    UserId = request.Source == "official" ? (int?) null : executionContext.UserId
                };

                uow.MapLayers.Add(mapLayer);
                await uow.SaveChangesAsync();

                return mapLayer;
            }
        }

        public async Task<MapMarker> CreateMapMarkerAsync(
            NaheulbookExecutionContext executionContext,
            int mapLayerId,
            MapMarkerRequest request
        )
        {
            using var uow = _unitOfWorkFactory.CreateUnitOfWork();

            var mapLayer = await uow.MapLayers.GetAsync(mapLayerId);
            if (mapLayer == null)
                throw new MapLayerNotFoundException(mapLayerId);

            await _authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapLayer);

            var mapMarker = new MapMarker
            {
                LayerId = mapLayerId,
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                MarkerInfo = _jsonUtil.SerializeNonNull(request.MarkerInfo)
            };

            uow.MapMarkers.Add(mapMarker);

            await uow.SaveChangesAsync();

            return mapMarker;
        }

        public async Task DeleteMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapMarkerId)
        {
            using var uow = _unitOfWorkFactory.CreateUnitOfWork();

            var mapMarker = await uow.MapMarkers.GetWithLayerAsync(mapMarkerId);
            if (mapMarker == null)
                throw new MapMarkerNotFoundException(mapMarkerId);

            await _authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapMarker.Layer);

            uow.MapMarkers.Remove(mapMarker);

            await uow.SaveChangesAsync();
        }

        public async Task<MapMarker> UpdateMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapMarkerId, MapMarkerRequest request)
        {
            using var uow = _unitOfWorkFactory.CreateUnitOfWork();

            var mapMarker = await uow.MapMarkers.GetWithLayerAsync(mapMarkerId);
            if (mapMarker == null)
                throw new MapMarkerNotFoundException(mapMarkerId);

            await _authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapMarker.Layer);

            mapMarker.Name = request.Name;
            mapMarker.Description = request.Description;
            mapMarker.Type = request.Type;
            mapMarker.MarkerInfo = _jsonUtil.SerializeNonNull(request.MarkerInfo);

            await uow.SaveChangesAsync();

            return mapMarker;
        }

        public async Task DeleteMapLayerAsync(NaheulbookExecutionContext executionContext, int mapLayerId)
        {
            using var uow = _unitOfWorkFactory.CreateUnitOfWork();

            var mapLayer = await uow.MapLayers.GetAsync(mapLayerId);
            if (mapLayer == null)
                throw new MapLayerNotFoundException(mapLayerId);

            await _authorizationUtil.EnsureCanEditMapLayerAsync(executionContext, mapLayer);

            uow.MapLayers.Remove(mapLayer);

            await uow.SaveChangesAsync();
        }

        public async Task<List<Map>> GetMapsAsync()
        {
            using var uow = _unitOfWorkFactory.CreateUnitOfWork();

            var maps = await uow.Maps.GetAllAsync();

            return maps;
        }
    }
}