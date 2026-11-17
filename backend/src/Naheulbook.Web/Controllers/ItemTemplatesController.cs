using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemTemplates")]
[ApiController]
public class ItemTemplatesController(IItemTemplateService itemTemplateService, IItemTemplateMapper itemTemplateMapper) : ControllerBase
{
    [HttpGet("{itemTemplateId:guid}")]
    public async Task<ActionResult<ItemTemplateResponse>> GetItemTemplateAsync(Guid itemTemplateId)
    {
        try
        {
            var itemTemplate = await itemTemplateService.GetItemTemplateAsync(itemTemplateId);

            return itemTemplateMapper.ToResponse(itemTemplate);
        }
        catch (ItemTemplateNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPut("{itemTemplateId:guid}")]
    public async Task<ActionResult<ItemTemplateResponse>> PutItemTemplateAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] Guid itemTemplateId,
        ItemTemplateRequest request
    )
    {
        try
        {
            var itemTemplate = await itemTemplateService.EditItemTemplateAsync(
                executionContext,
                itemTemplateId,
                request
            );

            return itemTemplateMapper.ToResponse(itemTemplate);
        }
        catch (ItemTemplateNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ItemTemplateResponse>>> GetSearchItemTemplateAsync(
        [FromServices] OptionalNaheulbookExecutionContext executionContext,
        [FromQuery] string filter
    )
    {
        var itemTemplates= await itemTemplateService.SearchItemTemplateAsync(filter, 40, executionContext.ExecutionExecutionContext?.UserId);
        return itemTemplates.Select(itemTemplateMapper.ToResponse).ToList();
    }

    [HttpPost]
    public async Task<JsonResult> PostCreateItemTemplateAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        ItemTemplateRequest request
    )
    {
        try
        {
            var itemTemplate = await itemTemplateService.CreateItemTemplateAsync(executionContext, request);
            var itemTemplateResponse = itemTemplateMapper.ToResponse(itemTemplate);
            return new JsonResult(itemTemplateResponse)
            {
                StatusCode = StatusCodes.Status201Created,
            };
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }
}