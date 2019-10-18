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

        [HttpGet("{MapId:int:min(1)}")]
        public async Task<ActionResult<MapResponse>> GetMapAsync(
            [FromRoute] int mapId
        )
        {
            try
            {
                var map = await _mapService.GetMapAsync(mapId);

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
            [ModelBinder(BinderType = typeof(JsonModelBinder), Name = "request")] CreateMapRequest request,
            [FromForm(Name = "image")] IFormFile image
        )
        {
            if (!image.ContentType.StartsWith("image/"))
                return BadRequest();

            var imageStream = image.OpenReadStream();
            var map = await _mapService.CreateMapAsync(executionContext, request, imageStream);

            return _mapper.Map<MapResponse>(map);
        }
    }
}