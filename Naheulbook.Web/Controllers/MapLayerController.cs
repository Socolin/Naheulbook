using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("/api/v2/mapLayers")]
    [ApiController]
    public class MapLayerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMapService _mapService;

        public MapLayerController(
            IMapper mapper,
            IMapService mapService
        )
        {
            _mapper = mapper;
            _mapService = mapService;
        }

        [HttpPost("{MapLayerId:int:min(1)}/markers")]
        public async Task<ActionResult<MapMarkerResponse>> PostCreateMapMarkerAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int mapLayerId,
            [FromBody] MapMarkerRequest request
        )
        {
            var map = await _mapService.CreateMapMarkerAsync(executionContext, mapLayerId, request);

            return _mapper.Map<MapMarkerResponse>(map);
        }
    }
}