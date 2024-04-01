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

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemTemplateSubCategories")]
[ApiController]
public class ItemTemplateSubCategoriesController(IItemTemplateSubCategoryService itemTemplateSubSubCategoryService, IMapper mapper)
    : ControllerBase
{
    [HttpPost]
    public async Task<CreatedActionResult<ItemTemplateSubCategoryResponse>> PostCreateItemTemplateSubCategoryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateItemTemplateSubCategoryRequest request
    )
    {
        try
        {
            var itemTemplateSubCategory = await itemTemplateSubSubCategoryService.CreateItemTemplateSubCategoryAsync(executionContext, request);
            return mapper.Map<ItemTemplateSubCategoryResponse>(itemTemplateSubCategory);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpGet("{ItemTemplateSubCategoryName}/itemTemplates")]
    public async Task<ActionResult<List<ItemTemplateResponse>>> GetItemTemplatesBySubCategoryTechNameAsync(
        [FromServices] OptionalNaheulbookExecutionContext executionContext,
        [FromRoute] string itemTemplateSubCategoryName,
        [FromQuery] bool includeCommunityItems = true
    )
    {
        try
        {
            var itemTemplates = await itemTemplateSubSubCategoryService.GetItemTemplatesBySubCategoryTechNameAsync(
                itemTemplateSubCategoryName,
                executionContext.ExecutionExecutionContext?.UserId,
                includeCommunityItems
            );
            return mapper.Map<List<ItemTemplateResponse>>(itemTemplates);
        }
        catch (ItemTemplateSubCategoryNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}