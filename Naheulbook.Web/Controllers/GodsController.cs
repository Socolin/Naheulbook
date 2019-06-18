using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/gods")]
    [ApiController]
    public class GodsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IGodService _godService;

        public GodsController(
            IMapper mapper,
            IGodService godService
        )
        {
            _mapper = mapper;
            _godService = godService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GodResponse>>> GetItemTemplateSectionsAsync()
        {
            var sections = await _godService.GetAllGodsAsync();
            return _mapper.Map<List<GodResponse>>(sections);
        }
    }
}