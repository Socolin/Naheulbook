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
    [Route("api/v2/itemTemplateCategories")]
    [ApiController]
    public class ItemTemplateCategoriesController : Controller
    {
        private readonly IItemTemplateCategoryService _itemTemplateCategoryService;
        private readonly IMapper _mapper;

        public ItemTemplateCategoriesController(IItemTemplateCategoryService itemTemplateCategoryService, IMapper mapper)
        {
            _itemTemplateCategoryService = itemTemplateCategoryService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<JsonResult> PostCreateCategoryAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateItemTemplateCategoryRequest request
        )
        {
            try
            {
                var itemTemplateCategory = await _itemTemplateCategoryService.CreateItemTemplateCategoryAsync(executionContext, request);
                var itemTemplateCategoryResponse = _mapper.Map<ItemTemplateCategoryResponse>(itemTemplateCategory);
                return new JsonResult(itemTemplateCategoryResponse)
                {
                    StatusCode = (int?) HttpStatusCode.Created
                };
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
        }

        [HttpGet("{ItemTemplateCategoryName}/itemTemplates")]
        public async Task<ActionResult<List<ItemTemplateResponse>>> GetItemTemplatesByCategoryTechNameAsync(
            [FromServices] OptionalNaheulbookExecutionContext executionContext,
            [FromRoute] string itemTemplateCategoryName,
            [FromQuery] bool includeCommunityItems = true
        )
        {
            try
            {
                var itemTemplates = await _itemTemplateCategoryService.GetItemTemplatesByCategoryTechNameAsync(
                    itemTemplateCategoryName,
                    executionContext.ExecutionExecutionContext?.UserId,
                    includeCommunityItems
                );
                return _mapper.Map<List<ItemTemplateResponse>>(itemTemplates);
            }
            catch (ItemTemplateCategoryNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }
    }
}