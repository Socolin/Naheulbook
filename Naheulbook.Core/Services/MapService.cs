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
        Task<Map> GetMapAsync(int mapId);
        Task<Map> CreateMapAsync(NaheulbookExecutionContext executionContext, CreateMapRequest request, Stream imageStream);
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

        public async Task<Map> GetMapAsync(int mapId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var map = await uow.Maps.GetAsync(mapId);
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
                catch (Exception ex)
                {
                    uow.Maps.Remove(map);
                }

                await uow.SaveChangesAsync();

                return map;
            }
        }
    }
}