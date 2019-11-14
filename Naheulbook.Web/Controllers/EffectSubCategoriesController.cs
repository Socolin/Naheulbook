using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/effectSubCategories")]
    [ApiController]
    public class EffectSubCategoriesController : ControllerBase
    {
        private readonly IEffectService _effectService;
        private readonly IMapper _mapper;

        public EffectSubCategoriesController(IEffectService effectService, IMapper mapper)
        {
            _effectService = effectService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<EffectTypeResponse>>> GetEffectSubCategoriesAsync()
        {
            var effectSubCategories = await _effectService.GetEffectSubCategoriesAsync();
            return _mapper.Map<List<EffectTypeResponse>>(effectSubCategories);
        }

        [HttpGet("{subCategoryId:int:min(1)}/effects")]
        public async Task<ActionResult<List<EffectResponse>>> GetEffectsAsync(long subCategoryId)
        {
            var effects = await _effectService.GetEffectsBySubCategoryAsync(subCategoryId);
            return _mapper.Map<List<EffectResponse>>(effects);
        }

        [HttpPost("{subCategoryId:int:min(1)}/effects")]
        public async Task<CreatedActionResult<EffectResponse>> PostCreateEffectAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int subCategoryId,
            CreateEffectRequest request
        )
        {
            try
            {
                var effect = await _effectService.CreateEffectAsync(executionContext, subCategoryId, request);
                return _mapper.Map<EffectResponse>(effect);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
        }

        [HttpPost]
        public async Task<CreatedActionResult<EffectSubCategoryResponse>> PostCreateEffectSubCategoryAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateEffectSubCategoryRequest request
        )
        {
            try
            {
                var effectSubCategory = await _effectService.CreateEffectSubCategoryAsync(executionContext, request);
                return _mapper.Map<EffectSubCategoryResponse>(effectSubCategory);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
        }
    }
}