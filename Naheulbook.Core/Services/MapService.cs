using System;
using System.IO;
using System.Linq;
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
        Task<MapMarker> CreateMapMarkerAsync(NaheulbookExecutionContext executionContext, int mapId, int mapLayerId, CreateMapMarkerRequest request);
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
                    Data = _jsonUtil.Serialize(mapData)
                };

                uow.Maps.Add(map);
                await uow.SaveChangesAsync();

                try
                {
                    var result = _mapImageUtil.SplitMapImage(imageStream, map.Id);
                    mapData.Width = result.Width;
                    mapData.Height = result.Height;
                    mapData.ZoomCount = result.ZoomCount;
                    mapData.ExtraZoomCount = result.ExtraZoomCount;

                    map.Data = _jsonUtil.Serialize(mapData);
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
            int mapId,
            int mapLayerId,
            CreateMapMarkerRequest request
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
                MarkerInfo = _jsonUtil.Serialize(request.MarkerInfo)
            };

            uow.MapMarkers.Add(mapMarker);

            await uow.SaveChangesAsync();

            return mapMarker;
        }
    }
}