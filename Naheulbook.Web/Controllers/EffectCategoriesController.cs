using System.Collections.Generic;
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
    [Route("api/v2/effectCategories")]
    [ApiController]
    public class EffectCategoriesController : Controller
    {
        private readonly IEffectService _effectService;
        private readonly IMapper _mapper;

        public EffectCategoriesController(IEffectService effectService, IMapper mapper)
        {
            _effectService = effectService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<EffectTypeResponse>>> GetEffectCategoriesAsync()
        {
            var effectCategories = await _effectService.GetEffectCategoriesAsync();
            return _mapper.Map<List<EffectTypeResponse>>(effectCategories);
        }

        [HttpGet("{categoryId:int:min(1)}/effects")]
        public async Task<ActionResult<List<EffectResponse>>> GetEffectsAsync(long categoryId)
        {
            var effects = await _effectService.GetEffectsByCategoryAsync(categoryId);
            return _mapper.Map<List<EffectResponse>>(effects);
        }

        [HttpPost]
        public async Task<JsonResult> PostCreateCategoryAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateEffectCategoryRequest request
        )
        {
            try
            {
                var effectCategory = await _effectService.CreateEffectCategoryAsync(executionContext, request);
                var effectCategoryResponse = _mapper.Map<EffectCategoryResponse>(effectCategory);
                return new JsonResult(effectCategoryResponse) {StatusCode = (int) HttpStatusCode.Created};
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
        }
    }
}