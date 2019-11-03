using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("/api/v2/maps")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMapService _mapService;

        public MapController(
            IMapper mapper,
            IMapService mapService
        )
        {
            _mapper = mapper;
            _mapService = mapService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MapSummaryResponse>>> GetMapListAsync()
        {
            var maps = await _mapService.GetMapsAsync();

            return _mapper.Map<List<MapSummaryResponse>>(maps);
        }

        [HttpGet("{MapId:int:min(1)}")]
        public async Task<ActionResult<MapResponse>> GetMapAsync(
            [FromServices] OptionalNaheulbookExecutionContext executionContext,
            [FromRoute] int mapId
        )
        {
            try
            {
                var map = await _mapService.GetMapAsync(mapId, executionContext.ExecutionExecutionContext?.UserId);

                return _mapper.Map<MapResponse>(map);
            }
            catch (MapNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<MapResponse>> PostCreateMapAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [ModelBinder(BinderType = typeof(JsonModelBinder), Name = "request")]
            CreateMapRequest request,
            [FromForm(Name = "image")] IFormFile image
        )
        {
            if (!image.ContentType.StartsWith("image/"))
                return BadRequest();

            var imageStream = image.OpenReadStream();
            var map = await _mapService.CreateMapAsync(executionContext, request, imageStream);

            return _mapper.Map<MapResponse>(map);
        }

        [HttpPut("{MapId:int:min(1)}")]
        public async Task<ActionResult<MapSummaryResponse>> PutUpdateMapAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int mapId,
            [FromBody] CreateMapRequest request
        )
        {
            var map = await _mapService.UpdateMapAsync(executionContext, mapId, request);

            return _mapper.Map<MapSummaryResponse>(map);
        }


        [HttpPost("{MapId:int:min(1)}/layers")]
        public async Task<ActionResult<MapLayerResponse>> PostCreateMapLayerAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int mapId,
            [FromBody] MapLayerRequest request
        )
        {
            var map = await _mapService.CreateMapLayerAsync(executionContext, mapId, request);

            return _mapper.Map<MapLayerResponse>(map);
        }
    }
}