using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/items")]
[ApiController]
public class ItemsController(IItemService itemTemplate, IMapper mapper) : ControllerBase
{
    [HttpPut("{ItemId}/data")]
    public async Task<ActionResult<ItemPartialResponse>> PutEditItemDataAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        ItemData itemData
    )
    {
        try
        {
            var item = await itemTemplate.UpdateItemDataAsync(executionContext, itemId, itemData);

            return mapper.Map<ItemPartialResponse>(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{ItemId}")]
    public async Task<ActionResult<ItemPartialResponse>> DeleteItemAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId
    )
    {
        try
        {
            await itemTemplate.DeleteItemAsync(executionContext, itemId);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{ItemId}/useCharge")]
    public async Task<ActionResult<ItemPartialResponse>> PostUseChargeAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        [FromBody] UseChargeItemRequest request
    )
    {
        try
        {
            var item = await itemTemplate.UseChargeAsync(executionContext, itemId, request);
            return mapper.Map<ItemPartialResponse>(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPut("{ItemId}/modifiers")]
    public async Task<ActionResult<ItemPartialResponse>> PutEditItemModifiersAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        IList<ActiveStatsModifier> itemModifiers
    )
    {
        try
        {
            var item = await itemTemplate.UpdateItemModifiersAsync(executionContext, itemId, itemModifiers);

            return mapper.Map<ItemPartialResponse>(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{ItemId}/equip")]
    public async Task<ActionResult<ItemPartialResponse>> PostEquipItemAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        EquipItemRequest request
    )
    {
        try
        {
            var item = await itemTemplate.EquipItemAsync(executionContext, itemId, request);

            return mapper.Map<ItemPartialResponse>(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPut("{ItemId}/container")]
    public async Task<ActionResult<ItemPartialResponse>> PutChangeItemContainerAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        ChangeItemContainerRequest request
    )
    {
        try
        {
            var item = await itemTemplate.ChangeItemContainerAsync(executionContext, itemId, request);

            return mapper.Map<ItemPartialResponse>(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{ItemId}/take")]
    public async Task<ActionResult<TakeItemResponse>> PostTakeItemAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        TakeItemRequest request
    )
    {
        try
        {
            var (takenItem, remainingQuantity) = await itemTemplate.TakeItemAsync(executionContext, itemId, request);

            return new TakeItemResponse
            {
                RemainingQuantity = remainingQuantity,
                TakenItem = mapper.Map<ItemResponse>(takenItem),
            };
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }


    [HttpPost("{ItemId}/give")]
    public async Task<ActionResult<GiveItemResponse>> PostGiveItemAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        GiveItemRequest request
    )
    {
        try
        {
            var remainingQuantity = await itemTemplate.GiveItemAsync(executionContext, itemId, request);

            return new GiveItemResponse
            {
                RemainingQuantity = remainingQuantity,
            };
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}