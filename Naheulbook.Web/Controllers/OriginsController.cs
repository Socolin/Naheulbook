using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/origins")]
    [ApiController]
    public class OriginsController : Controller
    {
        private readonly IOriginService _originService;
        private readonly IMapper _mapper;

        public OriginsController(IOriginService originService, IMapper mapper)
        {
            _originService = originService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<OriginResponse>>> GetAsync()
        {
            var skills = await _originService.GetOriginsAsync();

            return _mapper.Map<List<OriginResponse>>(skills);
        }
    }
}