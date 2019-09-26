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

        [HttpPost]
        public async Task<JsonResult> PostCreateTypeAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateEffectTypeRequest request
        )
        {
            try
            {
                var effectType = await _effectService.CreateEffectTypeAsync(executionContext, request);
                return new JsonResult(_mapper.Map<EffectTypeResponse>(effectType))
                {
                    StatusCode = 201
                };
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
        }
    }
}