using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/locations")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILocationService _locationService;

        public LocationsController(
            IMapper mapper,
            ILocationService locationService
        )
        {
            _mapper = mapper;
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<LocationResponse>>> GetLocationsAsync()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return _mapper.Map<List<LocationResponse>>(locations);
        }

        [HttpGet("{LocationId:int:min(1)}/maps")]
        public async Task<ActionResult<List<LocationMapResponse>>> GetLocationsAsync([FromRoute] int locationId)
        {
            var maps = await _locationService.GetLocationMapsAsync(locationId);
            return _mapper.Map<List<LocationMapResponse>>(maps);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<LocationResponse>>> GetSearchLocationAsync([FromQuery] string filter)
        {
            var locations = await _locationService.SearchLocationAsync(filter);
            return _mapper.Map<List<LocationResponse>>(locations);
        }
    }
}