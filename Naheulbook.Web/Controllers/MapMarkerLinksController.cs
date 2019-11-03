using System.Threading.Tasks;
using AutoMapper;
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
    [Route("/api/v2/mapMarkerLinks")]
    [ApiController]
    public class MapMarkerLinksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMapService _mapService;

        public MapMarkerLinksController(IMapService mapService, IMapper mapper)
        {
            _mapService = mapService;
            _mapper = mapper;
        }

        [HttpPut("{MapMarkerLinkId}")]
        public async Task<ActionResult<MapMarkerLinkResponse>> PutEditMapMarkerLinkAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int mapMarkerLinkId,
            MapMarkerLinkRequest request
        )
        {
            try
            {
                var mapMarkerLink = await _mapService.EditMapMarkerLinkAsync(executionContext, mapMarkerLinkId, request);
                return _mapper.Map<MapMarkerLinkResponse>(mapMarkerLink);
            }
            catch (MapMarkerNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpDelete("{MapMarkerLinkId}")]
        public async Task<IActionResult> DeleteMarkerLinkAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int mapMarkerLinkId
        )
        {
            try
            {
                await _mapService.DeleteMapMarkerLinkAsync(executionContext, mapMarkerLinkId);
            }
            catch (MapMarkerNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }

            return NoContent();
        }
    }
}