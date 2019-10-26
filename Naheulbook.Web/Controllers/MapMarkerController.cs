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
    [Route("/api/v2/mapMarkers")]
    [ApiController]
    public class MapMarkerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMapService _mapService;

        public MapMarkerController(IMapService mapService, IMapper mapper)
        {
            _mapService = mapService;
            _mapper = mapper;
        }

        [HttpDelete("{MapMarkerId}")]
        public async Task<IActionResult> DeleteMarkerAsync(
            [FromServices] NaheulbookExecutionContext naheulbookExecutionContext,
            [FromRoute] int mapMarkerId
        )
        {
            try
            {
                await _mapService.DeleteMapMarkerAsync(naheulbookExecutionContext, mapMarkerId);
            }
            catch (MapMarkerNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }

            return NoContent();
        }

        [HttpPut("{MapMarkerId}")]
        public async Task<ActionResult<MapMarkerResponse>> PutMarkerAsync(
            [FromServices] NaheulbookExecutionContext naheulbookExecutionContext,
            [FromRoute] int mapMarkerId,
            [FromBody] MapMarkerRequest request
        )
        {
            try
            {
                var marker = await _mapService.UpdateMapMarkerAsync(naheulbookExecutionContext, mapMarkerId, request);
                return _mapper.Map<MapMarkerResponse>(marker);
            }
            catch (MapMarkerNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }

            return NoContent();
        }
    }
}