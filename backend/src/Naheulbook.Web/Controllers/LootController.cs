using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Loot;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/loots")]
[ApiController]
public class LootController(
    ILootService lootService,
    IMapper mapper
) : ControllerBase
{
    [HttpPut("{LootId:int:min(1)}/visibility")]
    public async Task<IActionResult> PutLootVisibilityAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int lootId,
        PutLootVisibilityRequest request
    )
    {
        try
        {
            await lootService.UpdateLootVisibilityAsync(executionContext, lootId, request);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (LootNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{LootId:int:min(1)}")]
    public async Task<IActionResult> DeleteLootAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int lootId
    )
    {
        try
        {
            await lootService.DeleteLootAsync(executionContext, lootId);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (LootNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{LootId:int:min(1)}/items")]
    public async Task<CreatedActionResult<ItemResponse>> PostAddItemToLootInventoryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int lootId,
        CreateItemRequest request
    )
    {
        try
        {
            var item = await lootService.AddItemToLootAsync(executionContext, lootId, request);
            return mapper.Map<ItemResponse>(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (LootNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (ItemTemplateNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }

    [HttpPost("{LootId:int:min(1)}/addRandomItem")]
    public async Task<CreatedActionResult<ItemResponse>> PostAddRandomItemToLootInventoryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int lootId,
        CreateRandomItemRequest request
    )
    {
        try
        {
            var item = await lootService.AddRandomItemToLootAsync(executionContext, lootId, request);
            return mapper.Map<ItemResponse>(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (LootNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (ItemTemplateSubCategoryNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }
}