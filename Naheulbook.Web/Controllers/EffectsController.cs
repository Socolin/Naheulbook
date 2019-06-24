using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [ApiController]
    [Route("api/v2/effects")]
    public class EffectsController : Controller
    {
        private readonly IEffectService _effectService;
        private readonly IMapper _mapper;

        public EffectsController(IEffectService effectService, IMapper mapper)
        {
            _effectService = effectService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<JsonResult> PostCreateEffectAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateEffectRequest request
        )
        {
            try
            {
                var effect = await _effectService.CreateEffectAsync(executionContext, request);
                var effectResponse = _mapper.Map<EffectResponse>(effect);
                return new JsonResult(effectResponse)
                {
                    StatusCode = (int?) HttpStatusCode.Created
                };
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
        }

        [HttpPut("{effectId:int:min(1)}")]
        public async Task<StatusCodeResult> PutEditEffectAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            int effectId,
            CreateEffectRequest request
        )
        {
            try
            {
                await _effectService.EditEffectAsync(executionContext, effectId, request);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (EffectNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpGet("{effectId:int:min(1)}")]
        public async Task<ActionResult<EffectResponse>> GetEffectAsync(int effectId)
        {
            try
            {
                var effect = await _effectService.GetEffectAsync(effectId);
                var effectResponse = _mapper.Map<EffectResponse>(effect);
                return effectResponse;
            }
            catch (EffectNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }
    }
}