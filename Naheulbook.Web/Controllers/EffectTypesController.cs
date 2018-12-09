using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Filters;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/effectTypes")]
    [ApiController]
    public class EffectTypesController : Controller
    {
        private readonly IEffectService _effectService;
        private readonly IMapper _mapper;

        public EffectTypesController(IEffectService effectService, IMapper mapper)
        {
            _effectService = effectService;
            _mapper = mapper;
        }

        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        [HttpPost]
        public async Task<ActionResult<EffectTypeResponse>> PostCreateTypeAsync(CreateEffectTypeRequest request)
        {
            var effectType = await _effectService.CreateEffectTypeAsync(HttpContext.GetExecutionContext(), request);
            return new JsonResult(_mapper.Map<EffectTypeResponse>(effectType))
            {
                StatusCode = 201
            };
        }
    }
}